using com.cozyhome.Actors;
using UnityEngine;

public class WallJumpState : ActorState
{
    [SerializeField] private AnimationCurve FallTimeCurve;
    [SerializeField] private AnimationCurve GravityCurve;
    [SerializeField] private AnimationCurve TurnTimeCurve;
    [SerializeField] private float MaxAngularAdjustment = 60F;
    [SerializeField] private float JumpHeight = 5F;
    [SerializeField] private float ForwardJumpSpeed = 10F;
    [SerializeField] private float MaxRotationSpeed = 10F;
    [SerializeField] private float MaxMoveInfluence = 10F;
    [SerializeField] private float MaxHorizontalSpeed = 28F;

    private float InitialSpeed;
    private bool HoldingJump;

    public void Prepare(Vector3 Normal)
    {
        PlayerInput PlayerInput = Machine.GetPlayerInput;
        Transform ModelView = Machine.GetModelView;
        Transform CameraView = Machine.GetCameraView;
        ActorHeader.Actor Actor = Machine.GetActor;
        Animator Animator = Machine.GetAnimator;

        Vector2 Local = PlayerInput.GetRawMove;
        Vector3 Move = CameraView.rotation * new Vector3(Local[0], 0F, Local[1]);
        Move[1] = 0F;
        Move.Normalize();

        float AngularDifference = Vector3.SignedAngle(Normal, Move, Vector3.up);

        AngularDifference = Mathf.Min(AngularDifference, MaxAngularAdjustment);
        AngularDifference = Mathf.Max(AngularDifference, -MaxAngularAdjustment);
        /* Rotate our Normal based on our move direction */
        Normal = Quaternion.AngleAxis(AngularDifference, Vector3.up) * Normal;

        Vector3 Velocity = (Normal * ForwardJumpSpeed) + Vector3.up * Mathf.Sqrt(2F * JumpHeight * PlayerVars.GRAVITY);
        InitialSpeed = Velocity[1];

        ModelView.rotation = Quaternion.LookRotation(Normal, Vector3.up);

        /* Construct our Initial Velocity for our jump: */

        // 1. Take the normal and use that (probably the way to go here)
        // 2. notify animator

        Actor.SetVelocity(Velocity);
        Animator.SetTrigger("Jump");
        HoldingJump = true;
    }

    public override void Enter(ActorState prev) { }

    public override void Exit(ActorState next) { }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity) { }

    public override void Tick(float fdt)
    {
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        PlayerInput PlayerInput = Machine.GetPlayerInput;
        Animator Animator = Machine.GetAnimator;
        Transform ModelView = Machine.GetModelView;
        Transform CameraView = Machine.GetCameraView;
        ActorHeader.Actor Actor = Machine.GetActor;

        Vector2 Local = PlayerInput.GetRawMove;
        Vector3 Move = ActorStateHeader.ComputeMoveVector(Local, CameraView.rotation, Vector3.up);

        Vector3 Velocity = Actor.velocity;

        bool SquareTrigger = PlayerInput.GetSquareTrigger;

        float gravitational_pull = PlayerVars.GRAVITY;
        float YComp = Velocity[1];
        float percent = YComp / InitialSpeed;

        if (ActorStateHeader.Transitions.CheckGeneralLedgeTransition(
            Actor.position,
            Machine.GetModelView.forward,
            Actor.orientation,
            LedgeRegistry,
            Machine))
            return;
        else if (SquareTrigger)
        {
            if (Machine.GetFSM.TrySwitchState((ActorState next) =>
            {
                return ((DiveState)next).CheckDiveEligiblity();
            }, "Dive"))
                return;
        }
        else if (Actor.Ground.stable)
        {
            Machine.GetFSM.SwitchState("Ground");
            return;
        }
        else
        {
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

            HoldingJump &= PlayerInput.GetXButton;

            ActorStateHeader.AccumulateDeviatingGravity(ref Velocity,
                HoldingJump ? GravityCurve.Evaluate(percent) : 1.0F,
                fdt,
                gravitational_pull);

            Actor.SetVelocity(Velocity);
            Animator.SetFloat("Time", FallTimeCurve.Evaluate(percent));
            return;
        }
    }

    protected override void OnStateInitialize() { }

}
