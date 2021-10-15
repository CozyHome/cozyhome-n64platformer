using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Archetype;
using com.cozyhome.Timers;
using com.cozyhome.Vectors;
using UnityEngine;

public class LedgeState : ActorState
{

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve HangTimeCurve;
    [SerializeField] private TimerHeader.DeltaTimer LedgeTimer;
    private ArchetypeHeader.Archetype PlayerArchetype;
    private Vector3 ledge_position, mantle_position, hang_position;

    [Header("Executions")]
    [SerializeField] private ExecutionHeader.Actor.OnLedgeExecution OnLedgeExecution;

    protected override void OnStateInitialize()
    {
        PlayerArchetype = Machine.GetActor.GetArchetype();

        Machine.GetActorEventRegistry.Event_ActorFoundLedge += delegate
        {
            /* Set Ledge Values invokes our ledge event */
            OnLedgeExecution.Prepare(ledge_position, hang_position, mantle_position, Machine.GetModelView.rotation);
            Machine.GetChain.AddExecution(OnLedgeExecution);
        };
    }
    public override void Enter(ActorState prev) { LedgeTimer.Reset(); }

    public override void Exit(ActorState next) { }

    public void Prepare(Vector3 position, Vector3 newposition)
    {
        float height_percentage = .75F;

        Animator Animator = Machine.GetAnimator;
        Animator.SetInteger("Step", 0); // clear for mantle state
        Animator.SetFloat("Time", 0F); // ledge transition lerper
        Animator.SetTrigger("Hang"); // tell system to hang

        this.ledge_position = position;
        this.mantle_position = newposition;
        this.hang_position = ledge_position;

        hang_position += VectorHeader.ProjectVector(mantle_position - ledge_position, Vector3.up);
        hang_position -= Vector3.up * (PlayerArchetype.Height() * height_percentage);

        this.Machine.GetActorEventRegistry.Event_ActorFoundLedge?.Invoke(hang_position);
    }


    public override void Tick(float fdt)
    {
        Animator Animator = Machine.GetAnimator;

        bool XButton = Machine.GetPlayerInput.GetXTrigger;
        bool IsLedgeLerping = Machine.GetChain.IsExecutionActive(
            ExecutionHeader.Actor.ExecutionIndex.OnLedgeExecution);

        // are we attempting to mantle, and are we done lerping to the ledge position?
        if (XButton && !IsLedgeLerping)
        {
            Machine.GetFSM.SwitchState(
                (next) =>
                {
                    ((MantleState)next).Prepare(hang_position, mantle_position);
                }, "Mantle");
            return;
        }
        else
        {
            Animator.SetFloat("Time", HangTimeCurve.Evaluate(LedgeTimer.NormalizedElapsed));
            LedgeTimer.Accumulate(fdt);
        }
    }
    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public override void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public override void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }
}
