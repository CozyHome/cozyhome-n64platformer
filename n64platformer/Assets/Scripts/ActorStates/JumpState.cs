using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class JumpState : State
{
    [SerializeField] private float JumpHeight = 4F;
    [SerializeField] private Animator Animator;

    private ActorHeader.Actor PlayerActor;
    protected override void OnStateInitialize()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();
    }

    public override void Enter(string previous_key, State previous_state)
    {
        Vector3 Velocity = PlayerActor._velocity;

        Velocity += Vector3.up * Mathf.Sqrt(2F * 39.62F * JumpHeight);

        PlayerActor.SetVelocity(Velocity);
        PlayerActor.SetSnapEnabled(false);

        Animator.SetTrigger("Jump");
    }

    public override void Exit(string next_key, State next_state)
    {
        PlayerActor.SetSnapEnabled(true);
    }

    public override void Tick(float fdt)
    {
        Vector3 Velocity = PlayerActor._velocity;

        Velocity -= Vector3.up * 39.62F * fdt;

        PlayerActor.SetVelocity(Velocity);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        if(VectorHeader.Dot(PlayerActor._velocity, ground.normal) < 0F)
        {
            machine.SwitchCurrentState("Ground");   
            Animator.SetTrigger("Land");
        }
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {
    
    }
}
