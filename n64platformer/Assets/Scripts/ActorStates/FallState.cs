using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class FallState : ActorState
{
    protected override void OnStateInitialize()
    {

    }

    public override void Enter(ActorState prev)
    {
        Machine.GetAnimator.SetTrigger("Fall");
        Machine.GetActor.SetSnapEnabled(false);
    }

    public override void Exit(ActorState next)
    {
        Machine.GetAnimator.SetTrigger("Land");
        Machine.GetActor.SetSnapEnabled(true);
    }

    public override void Tick(float fdt)
    {
        ActorHeader.Actor Actor = Machine.GetActor;

        Vector3 Velocity = Actor._velocity;
        Velocity -= Vector3.up * (PlayerVars.GRAVITY * fdt);

        Actor.SetVelocity(Velocity);
    }


    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        if (VectorHeader.Dot(Machine.GetActor._velocity, ground.normal) < 0F)
            Machine.GetFSM.SwitchState("Ground");
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    { }
}
