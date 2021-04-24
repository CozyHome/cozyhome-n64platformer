using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public static class PlayerVars 
{
    public const float GRAVITY = 79.68F;
}

public class JumpState : State
{
    [SerializeField] private PlayerInput PlayerInput;

    [SerializeField] private float JumpHeight = 4F;
    [SerializeField] private Animator Animator;
    [SerializeField] private AnimationCurve FallTimeCurve;
    [SerializeField] private AnimationCurve GravityCurve;

    private float InitialSpeed;
    private bool HoldingJump = true;

    private ActorHeader.Actor PlayerActor;
    protected override void OnStateInitialize()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();
    }

    public override void Enter(string previous_key, State previous_state)
    {
        Vector3 Velocity = PlayerActor._velocity;

        InitialSpeed = Mathf.Sqrt(2F * PlayerVars.GRAVITY * JumpHeight);

        Velocity += Vector3.up * InitialSpeed;

        PlayerActor.SetVelocity(Velocity);
        PlayerActor.SetSnapEnabled(false);
        
        HoldingJump = true;

        Animator.SetTrigger("Jump");
    }

    public override void Exit(string next_key, State next_state)
    {
        PlayerActor.SetSnapEnabled(true);
        Animator.SetTrigger("Land");
    }

    public override void Tick(float fdt)
    {
        HoldingJump &= PlayerInput.GetXButton;

        Vector3 Velocity = PlayerActor._velocity;

        float gravitational_pull = PlayerVars.GRAVITY;
        float YComp = Velocity[1];
        float percent = YComp / InitialSpeed;

        if(HoldingJump)
            gravitational_pull = GravityCurve.Evaluate(percent) * PlayerVars.GRAVITY;

        Velocity -= Vector3.up * gravitational_pull * fdt;

        Animator.SetFloat("Time", FallTimeCurve.Evaluate(percent));

        PlayerActor.SetVelocity(Velocity);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        if (VectorHeader.Dot(PlayerActor._velocity, ground.normal) < 0F)
            machine.SwitchCurrentState("Ground");
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }
}
