using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Actors;
using System;

public class PlayerMachine : MonoBehaviour, ActorHeader.IActorReceiver
{
    private ActorHeader.Actor PlayerActor;
    private Dictionary<string, State> States = new Dictionary<string, State>();
    private State Current;

    void Start()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();
        State[] tmpbuffer = gameObject.GetComponents<State>();

        for(int i = 0;i< tmpbuffer.Length;i++)
            tmpbuffer[i].Initialize(this);
    }

    void FixedUpdate() 
    {

        PlayerActor.SetPosition(transform.position);
        PlayerActor.SetOrientation(transform.rotation);
        Current.Tick(Time.fixedDeltaTime);

        ActorHeader.Move(this, PlayerActor, Time.fixedDeltaTime);
    
        transform.SetPositionAndRotation(PlayerActor._position, PlayerActor.orientation);
    }

    public void SwitchCurrentState(string key) 
    {
        State next = States[key];

        // tell current state we are leaving
        Current.Exit(key, next);

        // tell next state we are entering
        next.Enter(Current.GetKey, Current);
        // swap states

        Current = next;
    }
    public void SetCurrentState(string key) => Current = States[key]; 
    public void AttachState(string key, State state) => States.Add(key, state);
    public void RemoveState(string key) => States.Remove(key);

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        Current.OnGroundHit(ground, lastground, layermask);
    }

    public void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {
        Current.OnTraceHit(trace, position, velocity);
    }
}

public abstract class State : MonoBehaviour, ActorHeader.IActorReceiver
{
    /* */
    [SerializeField] protected string Key;

    protected PlayerMachine machine;

    public void Initialize(PlayerMachine machine) 
    {
        this.machine = machine;
        machine.AttachState(Key, this);

        this.OnStateInitialize();
    }

    public string GetKey => Key;

    protected abstract void OnStateInitialize();

    public abstract void Enter(string previous_key, State previous_state);
    public abstract void Exit(string next_key, State next_state);
    public abstract void Tick(float fdt);

    public abstract void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask);
    public abstract void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity);
}