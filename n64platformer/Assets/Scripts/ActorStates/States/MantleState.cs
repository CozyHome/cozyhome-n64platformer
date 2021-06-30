using com.cozyhome.Actors;
using com.cozyhome.Timers;
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

    [SerializeField] private TimerHeader.DeltaTimer AnimationTimer;
    private Vector3 Displacement;
    private MantleType MantleType;

    public void Prepare(Vector3 hang_position, Vector3 mantle_position) /* Called when player presses XButton in LedgeState */
    {
        Displacement = mantle_position - hang_position;
        MantleType = MantleType.Fast;
    }

    protected override void OnStateInitialize() { }

    public override void Enter(ActorState prev)
    {
        AnimationTimer.Reset();

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
                AnimationTimer.Max(1F / 2F);
                break;
            case MantleType.Slow:
                Animator.SetInteger("Step", 2);
                AnimationTimer.Max(1F / 1.33F);
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
    public override void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public override void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }
    public override void Tick(float fdt)
    {
        Transform ModelView = Machine.GetModelView;
        Animator Animator = Machine.GetAnimator;
        ActorHeader.Actor Actor = Machine.GetActor;

        if (Actor.Ground.stable && AnimationTimer.Check())
        {
            Machine.GetFSM.SwitchState("Ground");
            Actor.SetVelocity(Vector3.zero);
            return;
        }
        else
            AnimationTimer.Accumulate(fdt);

        Vector3 AnimationVelocity = AnimationMoveBundle.GetRootDisplacement(fdt);
        AnimationMoveBundle.Clear();

        if (AnimationTimer.Elapsed > 0F && Mathf.Abs(AnimationVelocity[1]) <= 0.01F) // lingering but zero
            AnimationVelocity[1] = -5F; // move downward linearly (could accumulate gravity) would add more logic then necessary though

        Actor.SetVelocity(AnimationVelocity);

    }

    private void OnAnimatorMove()
    {
        Animator Animator = Machine.GetAnimator;

        AnimationMoveBundle.Accumulate(Animator.deltaPosition, Animator.deltaRotation);
    }
}
