using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public static class PlayerVars
{
    public const float GRAVITY = 79.68F;
}

public class JumpState : ActorState
{
    [Header("General References")]
    [SerializeField] private LedgeRegistry LedgeRegistry;

    [Header("Jump Properties")]
    [SerializeField] private float JumpHeight = 4F;
    [SerializeField] private float MaxLedgeVelocity = 1.0F;
    private float InitialSpeed;
    private bool HoldingJump = true;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve FallTimeCurve;
    [SerializeField] private AnimationCurve GravityCurve;


    protected override void OnStateInitialize()
    {

    }

    public override void Enter(ActorState prev)
    {
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor._velocity;

        InitialSpeed = Mathf.Sqrt(2F * PlayerVars.GRAVITY * JumpHeight);
        Velocity += Vector3.up * InitialSpeed;
        
        Actor.SetVelocity(Velocity);
        Actor.SetSnapEnabled(false);
        
        HoldingJump = true;
        Machine.GetAnimator.SetTrigger("Jump");
        
        /* notify our callback system */
        Machine.GetEventRegistry.Event_ActorJumped?.Invoke();
    }

    public override void Exit(ActorState next)
    {
        Machine.GetActor.SetSnapEnabled(true);
    }

    public override void Tick(float fdt)
    {
        ActorHeader.Actor Actor = Machine.GetActor;
        PlayerInput PlayerInput = Machine.GetPlayerInput;

        /* Continual Ledge Detection  */
        if (/* only do if falling */ VectorHeader.Dot(Machine.GetActor._velocity, Vector3.up) <= MaxLedgeVelocity &&
            LedgeRegistry.DetectLedge(
            LedgeRegistry.GetProbeDistance,
            Actor._position,
            Machine.GetModelView.forward,
            Actor.orientation,
            out Vector3 ledge_position))
        {
            Machine.GetFSM.SwitchState("Ledge");
            ((LedgeState)Machine.GetFSM.Current).Prepare(
                Actor._position,
                ledge_position
            );

            /* attach callback to process setting our initial values on time */
            return;
        }

        HoldingJump &= PlayerInput.GetXButton;

        Vector3 Velocity = Actor._velocity;

        float gravitational_pull = PlayerVars.GRAVITY;
        float YComp = Velocity[1];
        float percent = YComp / InitialSpeed;

        if (HoldingJump)
            gravitational_pull = GravityCurve.Evaluate(percent) * PlayerVars.GRAVITY;

        Velocity -= Vector3.up * gravitational_pull * fdt;

        Machine.GetAnimator.SetFloat("Time", FallTimeCurve.Evaluate(percent));

        Actor.SetVelocity(Velocity);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        if (VectorHeader.Dot(Machine.GetActor._velocity, ground.normal) < 0F)
        {
            Machine.GetFSM.SwitchState("Ground");
            Machine.GetAnimator.SetTrigger("Land");
        }
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {
        
    }

}
