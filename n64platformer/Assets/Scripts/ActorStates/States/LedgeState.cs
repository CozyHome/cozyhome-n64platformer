using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Archetype;
using com.cozyhome.Vectors;
using UnityEngine;

public class LedgeState : ActorState
{

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
    public override void Enter(ActorState prev)
    {

    }

    public override void Exit(ActorState next)
    {

    }

    public void Prepare(Vector3 position, Vector3 newposition)
    {
        Animator Animator = Machine.GetAnimator;
        Animator.SetTrigger("Hang");

        this.ledge_position = position;
        this.mantle_position = newposition;
        this.hang_position = ledge_position;

        hang_position += VectorHeader.ProjectVector(mantle_position - ledge_position, Vector3.up);
        hang_position -= Vector3.up * (PlayerArchetype.Height() * 0.8F);

        this.Machine.GetActorEventRegistry.Event_ActorFoundLedge?.Invoke(hang_position);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {

    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }

    public override void Tick(float fdt)
    {
        bool XButton = Machine.GetPlayerInput.GetXButton;

        if (XButton &&
            !Machine.GetChain.IsExecutionActive(ExecutionHeader.Actor.ExecutionIndex.OnLedgeExecution))
        {
            Machine.GetFSM.SwitchState(
            (next) =>
            {
                ((MantleState)next).Prepare(hang_position, mantle_position);
            }, "Mantle");
            return;
        }

        /*
        ModelView.rotation = Quaternion.LookRotation(
            VectorHeader.ClipVector(mantle_position - ledge_position, Vector3.up), 
            Vector3.up
        );

        PlayerActor.SetPosition(hang_position);
        PlayerActor.SetVelocity(Vector3.zero);
        */
    }
}
