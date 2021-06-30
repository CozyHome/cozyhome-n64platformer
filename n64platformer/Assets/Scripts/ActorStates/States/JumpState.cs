using com.cozyhome.Actors;
using com.cozyhome.Timers;
using com.cozyhome.Vectors;
using UnityEngine;

public static class PlayerVars
{
    public const float GRAVITY = 79.68F;
}

public class JumpState : ActorState
{

    [Header("Jump Properties")]
    [SerializeField] private float JumpHeight = 4F;
    private float InitialSpeed;
    private bool HoldingJump = true;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve FallTimeCurve;
    [SerializeField] private AnimationCurve GravityCurve;
    [SerializeField] private AnimationCurve TurnTimeCurve;
    [SerializeField] private float MaxRotationSpeed = 360F;
    [SerializeField] private float MaxMoveInfluence = 10F;
    [SerializeField] private float MaxHorizontalSpeed = 28F;

    [SerializeField] private TimerHeader.SnapshotTimer LastLandingTimer;
    private float LastJumpTilt = 0F;

    protected override void OnStateInitialize()
    {
        Machine.GetActorEventRegistry.Event_ActorLanded += delegate
        {
            LastLandingTimer.Stamp(Time.time);
        };
    }

    public override void Enter(ActorState prev) { }

    public override void Exit(ActorState next) { Machine.GetActor.SetSnapEnabled(true); }

    public void PrepareDefault()
    {
        Animator Animator = Machine.GetAnimator;
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor.velocity;

        InitialSpeed = Mathf.Sqrt(2F * PlayerVars.GRAVITY * JumpHeight);
        Velocity += Vector3.up * InitialSpeed;
        HoldingJump = true;

        Actor.SetVelocity(Velocity);
        Actor.SetSnapEnabled(false);

        /* swap jump poses */
        if (!LastLandingTimer.Check(Time.time))
            LastJumpTilt = (LastJumpTilt + 1) % 2;
        else
            LastJumpTilt = 0F;

        Animator.SetTrigger("Jump");
        Animator.SetFloat("Tilt", LastJumpTilt);

        /* notify our callback system */
        Machine.GetActorEventRegistry.Event_ActorJumped?.Invoke();
    }

    public override void Tick(float fdt)
    {
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        ActorHeader.Actor Actor = Machine.GetActor;
        PlayerInput PlayerInput = Machine.GetPlayerInput;
        Transform ModelView = Machine.GetModelView;
        Transform CameraView = Machine.GetCameraView;

        Vector2 Local = Machine.GetPlayerInput.GetRawMove;
        Vector3 Move = ActorStateHeader.ComputeMoveVector(Local, CameraView.rotation, ModelView.up);
        Vector3 Velocity = Actor.velocity;

        bool SquareTrigger = PlayerInput.GetSquareTrigger;

        HoldingJump &= PlayerInput.GetXButton;

        float gravitational_pull = PlayerVars.GRAVITY;
        float YComp = Velocity[1];
        float percent = YComp / InitialSpeed;

        /* Continual Ledge Detection  */
        if (ActorStateHeader.Transitions.CheckGeneralLedgeTransition(
            Actor.position,
            ModelView.forward,
            Actor.orientation,
            LedgeRegistry,
            Machine))
            return;
        else if (SquareTrigger)
        {
            if (Machine.GetFSM.TrySwitchState((ActorState next) =>
            {
                return ((DiveState) next).CheckDiveEligiblity();
            }, "Dive"))
                return;
        }
        else if (Actor.Ground.stable && Mathf.Abs(VectorHeader.Dot(Velocity, Actor.Ground.normal)) <= 0.1F)
        {
            Machine.GetFSM.SwitchState("Ground");
            return;
        }
        else
        {
            ActorStateHeader.AccumulateDeviatingGravity(ref Velocity,
                HoldingJump ? GravityCurve.Evaluate(percent) : 1.0F,
                fdt,
                gravitational_pull);

            /* Jump Repair */
            ActorStateHeader.RepairTime(
                fdt,
                TurnTimeCurve.Evaluate(percent),
                MaxRotationSpeed,
                MaxHorizontalSpeed,
                MaxMoveInfluence,
                Move,
                ModelView,
                ref Velocity);

            Actor.SetVelocity(Velocity);
            Machine.GetAnimator.SetFloat("Time", FallTimeCurve.Evaluate(percent));
        }
    }
    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public override void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public override void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }
}
