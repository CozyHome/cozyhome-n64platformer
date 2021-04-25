using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Actors;
using System;

public class PlayerMachine : MonoBehaviour, ActorHeader.IActorReceiver
{
    private ActorHeader.Actor PlayerActor;
    private MonoFSM<string, ActorState> FSM;
    private ActorEventRegistry EventRegistry;
    void Start()
    {
        FSM = new MonoFSM<string, ActorState>();

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

    public MonoFSM<string, ActorState> GetFSM => FSM;
    public ActorEventRegistry GetEventRegistry => EventRegistry;
}

public abstract class ActorState : MonoBehaviour, ActorHeader.IActorReceiver, MonoFSM<string, ActorState>.IMonoState
{
    /* */
    [SerializeField] protected string Key;

    protected PlayerMachine machine;

    public void Initialize(PlayerMachine machine)
    {
        this.machine = machine;
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