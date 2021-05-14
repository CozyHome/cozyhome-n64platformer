using com.cozyhome.Actors;
using UnityEngine;

public class WallJumpState : ActorState
{
    [SerializeField] private float JumpHeight = 5F;

    public void Prepare()
    {
        Transform ModelView = Machine.GetModelView;
        ActorHeader.Actor Actor = Machine.GetActor;
        Animator Animator = Machine.GetAnimator;

        Vector3 Velocity = Vector3.zero;

        /* Construct our Initial Velocity for our jump: */

        // 1. Take the normal and use that (probably the way to go here)
        // 2. notify animator

        Actor.SetVelocity(Velocity);

        Animator.SetTrigger("Jump");
    }

    public override void Enter(ActorState prev) { }

    public override void Exit(ActorState next) { }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity) { }

    public override void Tick(float fdt)
    {
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor.velocity;

        Velocity -= Vector3.up * PlayerVars.GRAVITY * fdt;

        Actor.SetVelocity(Velocity);
    }

    protected override void OnStateInitialize() { }

}
