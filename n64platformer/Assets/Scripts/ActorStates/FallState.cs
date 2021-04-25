using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class FallState : ActorState
{
    [SerializeField] private Animator Animator;

    private ActorHeader.Actor PlayerActor;

    protected override void OnStateInitialize()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();
    }

    public override void Enter(ActorState prev)
    {
        Animator.SetTrigger("Fall");
        PlayerActor.SetSnapEnabled(false);
    }

    public override void Exit(ActorState next)
    {
        Animator.SetTrigger("Land");
        PlayerActor.SetSnapEnabled(true);
    }

    public override void Tick(float fdt)
    {
        Vector3 Velocity = PlayerActor._velocity;
        Velocity -= Vector3.up * (PlayerVars.GRAVITY * fdt);

        PlayerActor.SetVelocity(Velocity);
    }


    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        if (VectorHeader.Dot(PlayerActor._velocity, ground.normal) < 0F)
            machine.GetFSM.SwitchState("Ground");
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    { }
}
