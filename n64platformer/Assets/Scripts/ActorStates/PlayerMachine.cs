using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Actors;
using System;

public class PlayerMachine : MonoBehaviour, ActorHeader.IActorReceiver
{
    [Header("General References")]
    [SerializeField] private Transform ModelView;
    [SerializeField] private Transform CameraView;
    [SerializeField] private Animator Animator;
    [SerializeField] private PlayerInput PlayerInput;

    private ActorHeader.Actor PlayerActor;
    private MonoFSM<string, ActorState> FSM;

    [Header("Events & Executions")]
    private ActorEventRegistry EventRegistry;
    [SerializeField] private ActorMiddleman MainMiddleman;
    private ExecutionChain<ExecutionHeader.Actor.ExecutionIndex, ActorMiddleman> MainChain;
    void Start()
    {
        FSM = new MonoFSM<string, ActorState>();
        MainChain = new ExecutionChain<ExecutionHeader.Actor.ExecutionIndex, ActorMiddleman>(MainMiddleman);

        EventRegistry = GetComponent<ActorEventRegistry>();
        PlayerActor = GetComponent<ActorHeader.Actor>();
        ActorState[] tmpbuffer = gameObject.GetComponents<ActorState>();

        for (int i = 0; i < tmpbuffer.Length; i++)
            tmpbuffer[i].Initialize(this);
    }

    void FixedUpdate()
    {
        PlayerActor.SetPosition(transform.position);
        PlayerActor.SetOrientation(transform.rotation);
        FSM.Current.Tick(Time.fixedDeltaTime);

        ActorHeader.Move(this, PlayerActor, Time.fixedDeltaTime);

        MainMiddleman.SetMachine(this);
        MainMiddleman.SetFixedDeltaTime(Time.fixedDeltaTime);
        MainChain.FixedTick();

        transform.SetPositionAndRotation(PlayerActor._position, PlayerActor.orientation);
    }

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        FSM.Current.OnGroundHit(ground, lastground, layermask);
    }

    public void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {
        FSM.Current.OnTraceHit(trace, position, velocity);
    }

    public ExecutionChain<ExecutionHeader.Actor.ExecutionIndex, ActorMiddleman> GetChain => MainChain;
    public MonoFSM<string, ActorState> GetFSM => FSM;
    public ActorEventRegistry GetEventRegistry => EventRegistry;

    public Transform GetModelView => ModelView;
    public Transform GetCameraView => CameraView;
    public Animator GetAnimator => Animator;
    public ActorHeader.Actor GetActor => PlayerActor;

    public PlayerInput GetPlayerInput => PlayerInput;
}

[System.Serializable]
public class ActorMiddleman
{
    public void SetFixedDeltaTime(float fdt) => this.fdt = fdt;
    public void SetMachine(PlayerMachine machine) => this.machine = machine;

    private float fdt;
    private PlayerMachine machine;

    public float FDT => fdt;
    public PlayerMachine Machine => machine;
}

public abstract class ActorState : MonoBehaviour, ActorHeader.IActorReceiver, MonoFSM<string, ActorState>.IMonoState
{
    /* */
    [SerializeField] protected string Key;

    protected PlayerMachine Machine;

    public void Initialize(PlayerMachine machine)
    {
        this.Machine = machine;
        machine.GetFSM.AddState(Key, this);

        this.OnStateInitialize();
    }

    public abstract void Enter(ActorState prev);
    public abstract void Exit(ActorState next);

    public string GetKey => Key;

    protected abstract void OnStateInitialize();

    public abstract void Tick(float fdt);

    public abstract void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask);
    public abstract void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity);

}