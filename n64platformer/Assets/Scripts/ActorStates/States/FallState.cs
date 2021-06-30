using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class FallState : ActorState
{
    protected override void OnStateInitialize() { }

    public override void Enter(ActorState prev)
    {
        Machine.GetAnimator.SetTrigger("Fall");
        Machine.GetActor.SetSnapEnabled(false);
    }

    public override void Exit(ActorState next)
    {
        Machine.GetActor.SetSnapEnabled(true);
    }

    public override void Tick(float fdt)
    {
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        ActorHeader.Actor Actor = Machine.GetActor;
        Transform ModelView = Machine.GetModelView;
        Vector3 Velocity = Actor.velocity;

        bool SquareTrigger = Machine.GetPlayerInput.GetSquareTrigger;

        /* Continual Ledge Detection  */
        if (ActorStateHeader.Transitions.CheckGeneralLedgeTransition(
            Actor.position,
            ModelView.forward,
            Actor.orientation,
            LedgeRegistry,
            Machine))
            return;
        else if (Actor.Ground.stable)
        {
            Machine.GetFSM.SwitchState("Ground");
            return;
        }
        else if (SquareTrigger)
        {
            Machine.GetFSM.SwitchState("Dive");
            return;
        }
        else
        {
            Velocity -= Vector3.up * (PlayerVars.GRAVITY * fdt);
            Actor.SetVelocity(Velocity);
            return;
        }
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }

    public override void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public override void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }
}
