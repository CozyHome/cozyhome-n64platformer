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
        Current.Tick();

        ActorHeader.Move(this, PlayerActor, Time.fixedDeltaTime);
    
        transform.SetPositionAndRotation(PlayerActor._position, PlayerActor.orientation);
    }

    public void SetCurrentState(string key) => Current = States[key]; 
    public void AttachState(string key, State state) => States.Add(key, state);
    public void RemoveState(string key) => States.Remove(key);

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
    
    }

    public void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {
    
    }
}

public abstract class State : MonoBehaviour
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
    
    protected abstract void OnStateInitialize();

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Tick();
}