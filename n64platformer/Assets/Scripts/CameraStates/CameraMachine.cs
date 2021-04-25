using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMachine : MonoBehaviour
{
    [SerializeField] private LayerMask clipmask;
    [SerializeField] private Transform ViewTransform;
    [SerializeField] private Transform OrbitTransform;
    [SerializeField] private float MaxVerticalAngle = 150F;
    [SerializeField] private float DolleyDistance = 16F;

    [SerializeField] private Vector3 WorldOffset;

    private Dictionary<string, CameraState> States = new Dictionary<string, CameraState>();
    private CameraState Current;

    void Start()
    {
        CameraState[] tmpbuffer = gameObject.GetComponents<CameraState>();

        for (int i = 0; i < tmpbuffer.Length; i++)
        {
            States.Add(tmpbuffer[i].GetKey, tmpbuffer[i]);
            tmpbuffer[i].Initialize(this);
        }
    }

    void FixedUpdate()
    {
        Current.FixedTick(Time.fixedDeltaTime);
    }
    void Update()
    {
        Current.Tick(Time.deltaTime);
    }

    public void SetCurrentState(string key) => Current = States[key];

    public void SwitchCurrentState(string key)
    {
        CameraState next = States[key];
        Current.Exit(key, next);
        next.Enter(Current.GetKey, Current);
        Current = next;
        // notify exit
        // notify enter
        // swap
    }

    public void SetViewRotation(Quaternion quaternion)
    {
        ViewTransform.rotation = quaternion;
    }

    public void ApplyOrbitPosition()
    {
        ViewTransform.position = WorldOffset + OrbitTransform.position - ViewTransform.forward * DolleyDistance;
    }

    public void ApplyOrbitPositionGround()
    {
        Vector3 dir = -Vector3.up;
        float dis = 7.5F;

        if (Physics.Raycast(ViewTransform.position,
        dir,
        out RaycastHit hit,
        dis,
        clipmask,
        QueryTriggerInteraction.Ignore))
        {
            ViewTransform.position = hit.point + Vector3.up * 5.0F;
        }
    }

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

    public void ComputeRealignments(
        ref Quaternion initial,
        ref Quaternion final)
    {
        Vector3 planarforward = ViewTransform.forward;
        planarforward[1] = 0F;
        planarforward.Normalize();

        float YAngle = Vector3.SignedAngle(planarforward, OrbitTransform.forward, Vector3.up);

        initial = ViewTransform.rotation;
        final = Quaternion.AngleAxis(YAngle, Vector3.up) * ViewTransform.rotation;
    }
}

public abstract class CameraState : MonoBehaviour
{
    [SerializeField] private string Key;
    protected CameraMachine machine;

    public void Initialize(CameraMachine machine)
    {
        this.machine = machine;


        this.OnInitialize();
    }

    protected abstract void OnInitialize();

    public abstract void Enter(string previous_key, CameraState previous_state);
    public abstract void Exit(string next_key, CameraState next_state);
    public abstract void FixedTick(float fdt);
    public abstract void Tick(float dt);

    public string GetKey => Key;
}