using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMachine : MonoBehaviour
{
    private MonoFSM<string, CameraState> FSM;

    [Header("Target References")]
    [SerializeField] private Transform ViewTransform;
    [SerializeField] private Transform OrbitTransform;
    [SerializeField] private ActorEventRegistry ActorEventRegistry;

    [Header("Target Values")]
    [SerializeField] private float DolleyDistance;
    [SerializeField] private float MaxVerticalAngle = 150F;

    /* Events */
    [Header("Event Subsystem References")]
    [SerializeField] private MainMiddleman Middleman;
    private ExecutionChain<int, MainMiddleman> MainChain;

    [SerializeField] private ExecutionHeader.OnJumpExecution JumpExecution;

    public float VerticalOffset = 4F;

    void Start()
    {
        FSM = new MonoFSM<string, CameraState>();
        MainChain = new ExecutionChain<int, MainMiddleman>(Middleman);

        AssignExecutions();

        CameraState[] tmpbuffer = GetComponents<CameraState>();
        for (int i = 0; i < tmpbuffer.Length; i++)
            tmpbuffer[i].Initialize(this);

    }

    void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        FSM.Current.FixedTick(fdt);
        
        Middleman.SetFixedDeltaTime(fdt);
        MainChain.FixedTick();
    }

    void Update()
    {
        FSM.Current.Tick(Time.deltaTime);
    }

    public MonoFSM<string, CameraState> GetFSM => FSM;
    public ActorEventRegistry GetEventRegistry => ActorEventRegistry;

    public void OrbitAroundTarget(Vector2 Input)
    {
        ViewTransform.rotation = Quaternion.AngleAxis(
                Input[0],
                Vector3.up
            ) * ViewTransform.rotation;

        float XAngle = Vector3.Angle(ViewTransform.forward, Vector3.up);
        float XDelta = Input[1];

        if (XDelta + XAngle > MaxVerticalAngle)
            XDelta = MaxVerticalAngle - XAngle;
        else if (XDelta + XAngle < 180F - MaxVerticalAngle)
            XDelta = (180F - MaxVerticalAngle) - XAngle;

        ViewTransform.rotation = Quaternion.AngleAxis(
            XDelta,
            ViewTransform.right
        ) * ViewTransform.rotation;
    }

    public void ApplyOrbitPosition(Vector3 offset)
    {
        ViewTransform.position =
            OrbitTransform.position - 
            (ViewTransform.forward * DolleyDistance) + (Vector3.up * VerticalOffset) + offset;
    }

    public void ApplyOrbitPosition()
    {
        ViewTransform.position =
            OrbitTransform.position - 
            (ViewTransform.forward * DolleyDistance) + (Vector3.up * VerticalOffset);
    }

    public void SetViewRotation(Quaternion newrotation)
    {
        ViewTransform.rotation = newrotation;
    }

    public void ComputeRealignments(ref Quaternion Initial, ref Quaternion Final)
    {
        Initial = ViewTransform.rotation;

        Vector3 planarforward = ViewTransform.forward;
        planarforward[1] = 0F;
        planarforward.Normalize();

        float YAngle = Vector3.SignedAngle(planarforward,
            OrbitTransform.forward,
            Vector3.up);

        Final = Quaternion.AngleAxis(YAngle, Vector3.up) * Initial;
    }

    private void AssignExecutions()
    {
        Middleman.SetMachine(this);

        GetEventRegistry.Event_ActorJumped += delegate
        {
            MainChain.AddExecution(JumpExecution);
        };
    }
}

public abstract class CameraState : MonoBehaviour, MonoFSM<string, CameraState>.IMonoState
{
    [SerializeField] private string Key;

    protected CameraMachine machine;

    public void Initialize(CameraMachine machine)
    {
        this.machine = machine;
        machine.GetFSM.AddState(Key, this);

        this.OnStateInitialize();
    }

    protected abstract void OnStateInitialize();

    public abstract void Enter(CameraState prev);

    public abstract void Exit(CameraState next);

    public abstract void Tick(float dt);
    public abstract void FixedTick(float fdt);

    public string GetKey => Key;
}

[System.Serializable]
public class MainMiddleman
{
    public void SetMachine(CameraMachine machine) => this.machine = machine;
    public void SetFixedDeltaTime(float fdt) => this.fdt = fdt;
    private CameraMachine machine;
    private float fdt;


    public CameraMachine Machine => machine;
    public float FDT => fdt;
}
