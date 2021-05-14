using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class WallSlideState : ActorState
{
    [SerializeField] private AnimationCurve GravitationalCurve;
    [SerializeField] private float VerticalLossPerSecond = 5F;
    [SerializeField] private float HorizontalLossPerSeccond = 3.5F;
    private Vector3 InitialVelocity;
    private float RightProduct;

    protected override void OnStateInitialize() { }

    public override void Enter(ActorState prev)
    {
        Animator Animator = Machine.GetAnimator;
        Animator.SetTrigger("Slide");
    }

    public override void Exit(ActorState next) { }

    public void Prepare(Vector3 wallnormal, Vector3 wallvelocity)
    {
        RightProduct = Vector3.SignedAngle(wallnormal, wallvelocity, Vector3.up) >= 0F ? 1F : -1F;
        Animator Animator = Machine.GetAnimator;
        Animator.SetFloat("Tilt", RightProduct);

        float wallproduct = VectorHeader.Dot(wallvelocity, wallnormal);

        Vector3 wallforward = wallvelocity - (wallnormal * wallproduct);
        wallforward[1] = 0F;
        wallforward.Normalize();

        Machine.GetModelView.rotation = Quaternion.LookRotation(wallforward, Vector3.up);
        InitialVelocity = wallvelocity;

        if (InitialVelocity[1] <= 0F)
            InitialVelocity[1] = 1F;

        /* somehow apply friction to horizontal velocity..? */
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        Machine.GetFSM.SwitchState(
                (next) =>
                {
                    Machine.GetAnimator.SetTrigger("Land");
                }, "Ground");
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity) { }

    public override void Tick(float fdt)
    {
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor.velocity;

        bool XTrigger = Machine.GetPlayerInput.GetXButton;

        /* Continual Ledge Detection  */
        if (ActorStateHeader.Transitions.CheckSlideTransitions(
            Actor.position,
            RightProduct * Machine.GetModelView.right,
            Actor.orientation,
            LedgeRegistry,
            Machine))
            return;
        else
        {
            /* Compute Horizontal & Vertical Velocity*/
            Vector3 HorizontalV = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));
            Vector3 VerticalV = Velocity - HorizontalV;

            HorizontalV *= (1F - (HorizontalLossPerSeccond * fdt));

            if (VerticalV[1] > 0F)
                VerticalV *= (1F - (VerticalLossPerSecond * fdt));

            VerticalV -= Vector3.up * PlayerVars.GRAVITY * GravitationalCurve.Evaluate(VerticalV[1] / InitialVelocity[1]) * fdt;

            Actor.SetVelocity(HorizontalV + VerticalV);

        }
    }

}
