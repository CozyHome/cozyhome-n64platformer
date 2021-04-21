using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Actors;

public class FallState : State
{
    private ActorHeader.Actor PlayerActor; 

    protected override void OnStateInitialize()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();

        machine.SetCurrentState(Key);
    }

    public override void Enter()
    {
    
    }

    public override void Exit()
    {
    
    }

    public override void Tick()
    {
        Vector3 Velocity = PlayerActor._velocity;

        Velocity -= new Vector3(0, 1, 0) * 39.62F * Time.fixedDeltaTime;

        PlayerActor.SetVelocity(Velocity);
    }

}
