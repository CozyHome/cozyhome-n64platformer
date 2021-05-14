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
    [SerializeField] private float GravitationalScale = 0.5F;
    [SerializeField] private float UpwardOffset;
    [SerializeField] private float InwardAcceleration;

    [Header("Animation Data")]
    [SerializeField] private AnimationMoveBundle AnimationMoveBundle;

    private Vector3 Displacement;
    private MantleType MantleType;

    public void Prepare(Vector3 hang_position, Vector3 mantle_position) /* Called when player presses XButton in LedgeState */
    {
        Displacement = mantle_position - hang_position;
        MantleType = MantleType.Fast;
    }

    protected override void OnStateInitialize()
    {

    }

    public override void Enter(ActorState prev)
    {
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
                break;
            case MantleType.Slow:
                Animator.SetInteger("Step", 2);
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

        float NormalizedTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        Vector3 AnimationVelocity = AnimationMoveBundle.GetRootDisplacement(fdt);
        AnimationMoveBundle.Clear();

        /* To give the player the ability to land on the platform in front of them, we'll apply an inward velocity every frame */
        Actor.SetVelocity(AnimationVelocity);

        if (Actor.Ground.stable && AnimationVelocity[1] <= 0F)
        {
            Machine.GetFSM.SwitchState(
            (ActorState next) =>
            {
                Machine.GetAnimator.SetTrigger("Land");
            }, "Ground");
        }
    }

    private void OnAnimatorMove()
    {
        Animator Animator = Machine.GetAnimator;

        AnimationMoveBundle.Accumulate(Animator.deltaPosition, Animator.deltaRotation);
    }
}
