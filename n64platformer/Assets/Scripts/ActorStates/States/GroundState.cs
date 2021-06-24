using com.cozyhome.Actors;
using com.cozyhome.Timers;
using com.cozyhome.Vectors;
using UnityEngine;

enum WalkType
{
    Idle = 0,
    Walk = 1,
    Run = 2
}

public class GroundState : ActorState
{
    // values
    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve RunRotationalCurve;
    [SerializeField] private AnimationCurve AnimatorSpeedCurve;
    [SerializeField] private AnimationCurve AccelerationCurve; // how fast we accelerate based on speed
    [SerializeField] private AnimationCurve DeaccelerationCurve; // how fast we deaccelerate based on speed
    [SerializeField] private AnimationCurve TiltCurve;
    [SerializeField] private float TiltSpeedAmount = 0.5F;
    [SerializeField] private float TiltSpeedInfluence = 0.5F;
    [SerializeField] private float TiltSpeedVelocity = 5F;

    [Header("Speeds & Rates")]
    [SerializeField] private float MaxRotateSpeed = 960F;
    [SerializeField] private float MaxMoveSpeed = 20F;
    [SerializeField] private float MoveAcceleration = 30F;
    [SerializeField] private float WalkAcceleration = 10F;

    [SerializeField] private TimerHeader.SnapshotTimer LastLandingTimer;
    private float TiltLerp = 0F;

    protected override void OnStateInitialize()
    {
        Machine.GetFSM.SetState(this.Key);

        Machine.GetActorEventRegistry.Event_ActorLanded += () =>
        {
            LastLandingTimer.Stamp(Time.time);
        };

    }

    public override void Enter(ActorState prev)
    {
        ActorEventRegistry EventRegistry = Machine.GetActorEventRegistry;
        Transform ModelView = Machine.GetModelView;
        Animator Animator = Machine.GetAnimator;

        EventRegistry.Event_ActorLanded?.Invoke();
        EventRegistry.Event_ActorTurn?.Invoke( ModelView.rotation );

        TiltLerp = 0F;
        Animator.SetTrigger("Land");
        Animator.ResetTrigger("Fall");
        Animator.SetFloat("Tilt", 0F);
        Animator.SetFloat("Time", 0F);
    }

    public override void Exit(ActorState next)
    {
        Animator Animator = Machine.GetAnimator;
        Animator.ResetTrigger("Land");
        Animator.SetFloat("Time", 0F);
        Animator.speed = 1F;
        TiltLerp = 0F;
    }

    public override void Tick(float fdt)
    {
        ActorEventRegistry EventRegistry = Machine.GetActorEventRegistry;
        Transform ModelView = Machine.GetModelView;
        Transform CameraView = Machine.GetCameraView;
        ActorHeader.Actor Actor = Machine.GetActor;
        PlayerInput PlayerInput = Machine.GetPlayerInput;
        Animator Animator = Machine.GetAnimator;

        bool XButton = PlayerInput.GetXTrigger;
        bool SquareTrigger = PlayerInput.GetSquareTrigger;

        Vector2 Local = PlayerInput.GetRawMove;
        Vector3 Move = ActorStateHeader.ComputeMoveVector(Local, CameraView.rotation, Vector3.up);
        Vector3 Velocity = Actor.velocity;

        float JoystickAmount = Local.magnitude;
        float Speed = Velocity.magnitude;
        float Ratio = Speed / MaxMoveSpeed;
        float NewTilt = 0F;

        if (DetermineTransitions(XButton, SquareTrigger, Actor))
            return;

        switch (GetWalkType(JoystickAmount))
        {
            case WalkType.Idle:

                if (JoystickAmount > 0.125F)
                    ModelView.rotation = Quaternion.LookRotation(Move, Vector3.up);

                Speed -= DeaccelerationCurve.Evaluate(Ratio) * fdt * MoveAcceleration;
                Speed = Mathf.Max(Speed, 0F);
                NewTilt = 0F;

                Animator.speed = 1F;

                break;
            case WalkType.Walk:

                NewTilt = MoveRotate(Move, MaxRotateSpeed * fdt);
                Speed = Mathf.Lerp(Speed, JoystickAmount * MaxMoveSpeed, WalkAcceleration * JoystickAmount * fdt);

                Animator.speed = 1F;

                break;

            case WalkType.Run:

                NewTilt = MoveRotate(Move, RunRotationalCurve.Evaluate(Ratio) * MaxRotateSpeed * fdt);
                TiltLerp = Mathf.Lerp(TiltLerp, NewTilt, TiltSpeedVelocity * fdt);

                Speed += AccelerationCurve.Evaluate(Ratio) * fdt * MoveAcceleration;
                Speed = Mathf.Min(Speed, MaxMoveSpeed);

                Animator.speed = AnimatorSpeedCurve.Evaluate(Ratio) + (Mathf.Abs(TiltLerp) * TiltSpeedInfluence);

                break;
        }


        Velocity = ModelView.rotation * new Vector3(0, 0, 1F);
        VectorHeader.CrossProjection(ref Velocity, Vector3.up, Actor.Ground.normal);

        Actor.SetVelocity(Velocity * Speed);

        Animator.SetFloat("Tilt", TiltLerp);
        Animator.SetFloat("Speed", Speed / MaxMoveSpeed);

        EventRegistry.Event_ActorTurn?.Invoke(ModelView.rotation);
    }

    private bool DetermineTransitions(bool XButton, bool SquareTrigger, ActorHeader.Actor Actor)
    {
        if (!Actor.Ground.stable)
        {
            Machine.GetFSM.SwitchState("Fall");
            return true;
        }

        if (SquareTrigger && LastLandingTimer.Check(Time.time))
        {
            if (Machine.GetFSM.TrySwitchState((ActorState next) =>
            {
                return ((DiveState)next).CheckDiveEligiblity();
            }, "Dive"))
                return true;
        }

        if (XButton)
        {
            Machine.GetFSM.SwitchState(
                (ActorState next) =>
                {
                    ((JumpState)next).PrepareDefault();
                }, "Jump");

            return true;
        }

        return false;
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    { }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    { }

    private float MoveRotate(Vector3 move, float rate)
    {
        Quaternion Old = Machine.GetModelView.rotation;

        Machine.GetModelView.rotation = Quaternion.RotateTowards(
                Machine.GetModelView.rotation,
                Quaternion.LookRotation(move, Vector3.up),
                rate);

        float YAngle = Vector3.SignedAngle(
            Old * Vector3.forward,
            Machine.GetModelView.forward,
            Vector3.up
        );

        return YAngle * TiltSpeedAmount;
    }

    private WalkType GetWalkType(float amount)
    {
        if (amount <= 0.25F)
            return WalkType.Idle;
        else if (amount < 0.45F)
            return WalkType.Walk;
        else
            return WalkType.Run;
    }

}
