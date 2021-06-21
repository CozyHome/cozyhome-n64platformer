using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

enum MantleType
{
    Fast = 0,
    Slow = 1
};

public class MantleState : ActorState
{
    [Header("Mantle Parameters")]
    [SerializeField] private float UpwardOffset;
    [SerializeField] private float InwardAcceleration;

    [Header("Animation Data")]
    [SerializeField] private AnimationMoveBundle AnimationMoveBundle;

    private Vector3 Displacement;
    private MantleType MantleType;
    private float TotalAnimationLength, AnimationElapsed;

    public void Prepare(Vector3 hang_position, Vector3 mantle_position) /* Called when player presses XButton in LedgeState */
    {
        Displacement = mantle_position - hang_position;
        MantleType = MantleType.Fast;
    }

    protected override void OnStateInitialize() { }

    public override void Enter(ActorState prev)
    {
        AnimationElapsed = 0F;

        ActorHeader.Actor Actor = Machine.GetActor;
        Animator Animator = Machine.GetAnimator;
        float UpwardVelocity = Displacement[1];

        /* Events: */
        AnimatorEventRegistry AnimatorEventRegistry = Machine.GetAnimatorEventRegistry;
        AnimatorEventRegistry.Event_AnimatorMove += OnAnimatorMove;

        AnimationMoveBundle.Clear();

        /* Mantle Type */
        switch (MantleType)
        {
            case MantleType.Fast:
                Animator.SetInteger("Step", 1);
                TotalAnimationLength = (1F / 2F);
                break;
            case MantleType.Slow:
                Animator.SetInteger("Step", 2);
                TotalAnimationLength = (1F / 1.33F);
                break;
        }

        Animator.SetFloat("Time", 0F);
        Actor.SetSnapEnabled(false);
    }

    public override void Exit(ActorState next)
    {
        ActorHeader.Actor Actor = Machine.GetActor;
        Animator Animator = Machine.GetAnimator;

        /* Events: */
        AnimatorEventRegistry AnimatorEventRegistry = Machine.GetAnimatorEventRegistry;
        AnimatorEventRegistry.Event_AnimatorMove -= OnAnimatorMove;

        Actor.SetSnapEnabled(true);
        Machine.GetAnimator.SetInteger("Step", 0);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity) { }

    public override void Tick(float fdt)
    {
        Transform ModelView = Machine.GetModelView;
        Animator Animator = Machine.GetAnimator;
        ActorHeader.Actor Actor = Machine.GetActor;

        if (Actor.Ground.stable && AnimationElapsed >= TotalAnimationLength)
        {
            Machine.GetFSM.SwitchState("Ground");
            Actor.SetVelocity(Vector3.zero);
            return;
        }
        else
            AnimationElapsed += fdt;

        Vector3 AnimationVelocity = AnimationMoveBundle.GetRootDisplacement(fdt);
        AnimationMoveBundle.Clear();

        /* To give the player the ability to land on the platform in front of them, we'll apply an inward velocity every frame */
        Actor.SetVelocity(AnimationVelocity);

    }

    private void OnAnimatorMove()
    {
        Animator Animator = Machine.GetAnimator;

        AnimationMoveBundle.Accumulate(Animator.deltaPosition, Animator.deltaRotation);
    }
}
