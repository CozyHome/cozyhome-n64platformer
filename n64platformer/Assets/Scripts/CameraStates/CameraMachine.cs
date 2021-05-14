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
    [SerializeField] private OccludeRegistry OccludeRegistry;

    [Header("Target Values")]
    [SerializeField] private AnimationCurve DistanceCurve;
    [SerializeField] private float DolleyDistance;
    [SerializeField] private float MaxVerticalAngle = 150F;

    /* Events */
    [Header("Event Subsystem References")]
    [SerializeField] private CameraMiddleman Middleman;
    private ExecutionChain<int, CameraMiddleman> MainChain;

    [SerializeField] private ExecutionHeader.Camera.OnJumpExecution JumpExecution;
    [SerializeField] private ExecutionHeader.Camera.OnHangExecution HangExecution;

    public float VerticalOffset = 4F;

    void Start()
    {
        FSM = new MonoFSM<string, CameraState>();
        MainChain = new ExecutionChain<int, CameraMiddleman>(Middleman);

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
        SolveOcclusion();
    }

    void Update()
    {
        FSM.Current.Tick(Time.deltaTime);
    }

    private void AssignExecutions()
    {
        Middleman.SetMachine(this);

        GetEventRegistry.Event_ActorJumped += () =>
        {
            MainChain.AddExecution(JumpExecution);
        };

        GetEventRegistry.Event_ActorFoundLedge += (Vector3 hang_position) => 
        {
            HangExecution.Prepare(Middleman, hang_position);
            MainChain.AddExecution(HangExecution);
        };
    }

    public MonoFSM<string, CameraState> GetFSM => FSM;
    public ActorEventRegistry GetEventRegistry => ActorEventRegistry;

    public Vector3 ViewPosition => ViewTransform.position;

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

    public void ApplyOrbitPosition() => ViewTransform.position = ComputeOrbitPosition();

    public Vector3 ComputeOrbitPosition()
    {
        float Ratio = Vector3.Dot( ViewTransform.forward, Vector3.up);
        float Amount = DistanceCurve.Evaluate(Ratio) * DolleyDistance;
 
        return OrbitTransform.position - (ViewTransform.forward * Amount) + (Vector3.up * VerticalOffset);
    }

    public void ApplyOffset(Vector3 offset) => ViewTransform.position += offset;
    public void SetViewPosition(Vector3 position) => ViewTransform.position = position;
    public void SetViewRotation(Quaternion newrotation) => ViewTransform.rotation = newrotation;
    
    public void SolveOcclusion() 
    {
        Vector3 start = OrbitTransform.position + (Vector3.up * VerticalOffset);
        Vector3 displacement = ViewTransform.position - start;
        float rto = OccludeRegistry.DetermineOcclusionRatio(start, displacement);
        ViewTransform.position = start + displacement * rto;
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
}

public abstract class CameraState : MonoBehaviour, MonoFSM<string, CameraState>.IMonoState
{
    [SerializeField] private string Key;

    protected CameraMachine Machine;

    public void Initialize(CameraMachine machine)
    {
        this.Machine = machine;
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
public class CameraMiddleman
{
    public void SetMachine(CameraMachine Machine) => this.Machine = Machine;
    public void SetFixedDeltaTime(float fdt) => this.fdt = fdt;
    private CameraMachine Machine;
    private float fdt;

    public CameraMachine GetMachine => this.Machine;
    public float FDT => fdt;
}
