using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using UnityEngine;

public class DiveSlideState : ActorState
{
    [SerializeField] private AnimationCurve FrictionCurve;
    [SerializeField] private float FrictionAmount = 10F;
    private float InitialSpeed;

    protected override void OnStateInitialize() { }

    public override void Enter(ActorState prev)
    {
        Animator Animator = Machine.GetAnimator;
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor.velocity;

        Vector3 HorizontalVelocity = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));
        InitialSpeed = HorizontalVelocity.magnitude;
    }

    public override void Exit(ActorState next) { }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public override void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public override void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }
    public override void Tick(float fdt)
    {
        Animator Animator = Machine.GetAnimator;
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor.velocity;

        Vector3 HorizontalVelocity = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));
        Vector3 VerticalVelocity = Velocity - HorizontalVelocity;
        float horizontal_len = HorizontalVelocity.magnitude;

        bool XTrigger = Machine.GetPlayerInput.GetXTrigger;

        if (!Actor.Ground.stable)
        {
            Machine.GetFSM.SwitchState("Fall");
            return;
        }
        else
        {
            if (XTrigger)
            {
                HandleTransitions(horizontal_len);
                return;
            }

            if (horizontal_len - (FrictionAmount * fdt) > 0F)
                HorizontalVelocity *= (horizontal_len - (FrictionAmount * fdt)) / horizontal_len;
            else
                HorizontalVelocity = Vector3.zero;

            Actor.SetVelocity(HorizontalVelocity + VerticalVelocity);
            Animator.SetFloat("Time", FrictionCurve.Evaluate(horizontal_len / InitialSpeed));
            return;
        }
    }

    private void HandleTransitions(float horizontal_len)
    {
        const float min_move_amount = 0.1F;

        if (horizontal_len > min_move_amount) // dive flip 
            Machine.GetFSM.SwitchState("DiveFlip");
        else // dive lift
            Machine.GetFSM.SwitchState("DiveLift");
    }
}
