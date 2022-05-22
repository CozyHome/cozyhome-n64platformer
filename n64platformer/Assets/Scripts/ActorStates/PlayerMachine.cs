using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using com.cozyhome.ChainedExecutions;
using com.cozyhome.Singleton;
using com.cozyhome.Systems;
using com.cozyhome.Console;
using com.cozyhome.Actors;

[DefaultExecutionOrder(100)]
public class PlayerMachine : MonoBehaviour, ActorHeader.IActorReceiver
{
    [Header("General References")]
    [SerializeField] private Transform      ModelView;
    [SerializeField] private Transform      CameraView;
    [SerializeField] private PlayerInput    PlayerInput;
    [SerializeField] private LedgeRegistry  LedgeRegistry;

    private Vector3 spawnposition;
    private ActorHeader.Actor PlayerActor;
    private Animator Animator;
    private MonoFSM<string, ActorState> FSM;

    [Header("Events & Executions")]
    private ActorEventRegistry ActorEventRegistry;
    private AnimatorEventRegistry AnimatorEventRegistry;
    [SerializeField] private ActorMiddleman MainMiddleman;
    private ExecutionChain<ExecutionHeader.Actor.ExecutionIndex, ActorMiddleman> MainChain;

    void Start()
    {
        spawnposition = transform.position;

        MonoConsole.InsertCommand("act_sw", Func_ActorSwitchState);
        MonoConsole.InsertCommand("act_rpos", Func_ActorResetPosition);

        /* Reference Initialization */
        FSM = new MonoFSM<string, ActorState>();
        MainChain = new ExecutionChain<ExecutionHeader.Actor.ExecutionIndex, ActorMiddleman>(MainMiddleman);

        PlayerActor = GetComponent<ActorHeader.Actor>();
        Animator = GetComponentInChildren<Animator>();

        ActorEventRegistry = GetComponent<ActorEventRegistry>();
        AnimatorEventRegistry = GetComponentInChildren<AnimatorEventRegistry>();

        /* State Initialization & Registration */
        ActorState[] tmpbuffer = gameObject.GetComponents<ActorState>();

        for (int i = 0; i < tmpbuffer.Length; i++)
            tmpbuffer[i].Initialize(this);
    }

    public void F_Update()
    {
        PlayerActor.SetPosition(transform.position);
        PlayerActor.SetOrientation(transform.rotation);
        FSM.Current.Tick(Time.fixedDeltaTime);
        ActorHeader.Move(this, PlayerActor, Time.fixedDeltaTime);

        MainMiddleman.SetMachine(this);
        MainMiddleman.SetFixedDeltaTime(Time.fixedDeltaTime);
        MainChain.Tick();

        transform.SetPositionAndRotation(PlayerActor.position, PlayerActor.orientation);
        Animator.Update(Time.fixedDeltaTime);
    }

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) {
        FSM.Current.OnGroundHit(ground, lastground, layermask);
        Debug.DrawRay(ground.point, ground.normal * 1000F, Color.green);

        if(Input.GetKey(KeyCode.Q))
            Debug.Break();        
    }

    public void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity){ 
        FSM.Current.OnTraceHit(tracetype, trace, position, velocity);
        // Debug.Log(tracetype + " " + trace.normal + " " + trace.point + " " + GetActor.DeterminePlaneStability(trace.normal, trace.collider));
        Debug.DrawRay(trace.point, trace.normal * 1000F, Color.magenta);

        if(Input.GetKey(KeyCode.Q))
            Debug.Break();
    }

    public void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger)
        => FSM.Current.OnTriggerHit(triggertype, trigger);

    public bool ValidGroundTransition(Vector3 normal, Collider collider) => PlayerActor.DeterminePlaneStability(normal, collider);

    public ExecutionChain<ExecutionHeader.Actor.ExecutionIndex, ActorMiddleman> GetChain => MainChain;
    public MonoFSM<string, ActorState> GetFSM => FSM;
    public ActorEventRegistry GetActorEventRegistry => ActorEventRegistry;
    public AnimatorEventRegistry GetAnimatorEventRegistry => AnimatorEventRegistry;
    public LedgeRegistry GetLedgeRegistry => LedgeRegistry;
    public Transform GetModelView => ModelView;
    public Transform GetCameraView => CameraView;
    public Animator GetAnimator => Animator;
    public ActorHeader.Actor GetActor => PlayerActor;
    public PlayerInput GetPlayerInput => PlayerInput;

    public void Func_ActorSwitchState(string[] modifiers, out string output)
    {
        output = "failed to switch state";

        if (modifiers.Length >= 1)
        {
            string nextkey = modifiers[0];
            if (string.IsNullOrEmpty(modifiers[0]))
                return;
            else if (FSM.TrySwitchState((ActorState state) => state != null, nextkey))
                    output = "actor state successfully switched to " + modifiers[0];

            return;
        }
    }
    public void Func_ActorResetPosition(string[] modifiers, out string output)
    {
        output = "Reset Actor position";
        this.transform.position = (spawnposition);
    }
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

public abstract class ActorState : MonoBehaviour, 
    ActorHeader.IActorReceiver, 
    MonoFSM<string, ActorState>.IMonoState
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
    public abstract void Tick(float fdt);
    public abstract void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask);
    public abstract void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity);
    public abstract void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger);
    protected abstract void OnStateInitialize();
}