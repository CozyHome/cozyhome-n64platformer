using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class WallSlideState : ActorState
{
    [SerializeField] private AnimationCurve GravitationalCurve;
    [SerializeField] private float HorizontalLossPerSecond = 3.5F;
    [SerializeField] private float MaxVerticalFallingSpeed = 8F;
    private Vector3 InitialVelocity;
    private float RightProduct;

    protected override void OnStateInitialize() { }

    public override void Enter(ActorState prev) {
        Debug.Log(prev.GetKey);
    }

    public override void Exit(ActorState next)
    {
        Animator Animator = Machine.GetAnimator;
        Animator.ResetTrigger("Slide");
    }

    public void Prepare(Vector3 wallnormal, Vector3 wallvelocity)
    {
        Animator Animator = Machine.GetAnimator;

        RightProduct = Vector3.SignedAngle(wallnormal, wallvelocity, Vector3.up) >= 0F ? 1F : -1F;
        Animator.SetTrigger("Slide");
        Animator.SetFloat("Tilt", RightProduct);

        Machine.GetModelView.rotation = Quaternion.LookRotation(wallnormal, Vector3.up);
        InitialVelocity = wallvelocity;

        if (InitialVelocity[1] <= 0F)
            InitialVelocity[1] = 1F;
    }

    public override void Tick(float fdt)
    {
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor.velocity;

        bool XTrigger = Machine.GetPlayerInput.GetXTrigger;

        /* Continual Ledge Detection */
        if (ActorStateHeader.Transitions.CheckSlideTransitions(
            Actor.position,
            -Machine.GetModelView.forward,
            Actor.orientation,
            LedgeRegistry,
            Machine))
            return;
        else if (Actor.Ground.stable)
        {
            Machine.GetFSM.SwitchState("Ground");
            return;
        }
        else if (XTrigger)
        {
            Machine.GetFSM.SwitchState(
                (ActorState next) =>
                {
                    ((WallJumpState)next).Prepare(Machine.GetModelView.forward);
                }, "WallJump");
        }
        else
        {
            /* Compute Horizontal & Vertical Velocity */
            Vector3 HorizontalV = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));
            Vector3 VerticalV = Velocity - HorizontalV;

            // HorizontalV *= (1F - (HorizontalLossPerSecond * fdt));
            VerticalV -= Vector3.up * PlayerVars.GRAVITY * GravitationalCurve.Evaluate(VerticalV[1] / InitialVelocity[1]) * fdt;

            if (VerticalV[1] <= -MaxVerticalFallingSpeed)
                VerticalV[1] *= (-MaxVerticalFallingSpeed / VerticalV[1]);

            Actor.SetVelocity(HorizontalV + VerticalV);
        }
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public override void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public override void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }
}
