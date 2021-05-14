using com.cozyhome.Actors;
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
    [SerializeField] private float MaxLedgeVelocity = 1.0F;
    private float InitialSpeed;
    private bool HoldingJump = true;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve FallTimeCurve;
    [SerializeField] private AnimationCurve GravityCurve;
    [SerializeField] private AnimationCurve TurnTimeCurve;
    [SerializeField] private float MaxRotationSpeed = 360F;
    [SerializeField] private float MaxMoveInfluence = 10F;
    [SerializeField] private float MaxHorizontalSpeed = 28F;

    private float LastLandTime = 0F;
    private float LastJumpTilt = 0F;

    protected override void OnStateInitialize()
    {
        Machine.GetActorEventRegistry.Event_ActorLanded += delegate
        {
            LastLandTime = Time.time;
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

        Actor.SetVelocity(Velocity);
        Actor.SetSnapEnabled(false);

        HoldingJump = true;
        Animator.SetTrigger("Jump");
        /* swap jump poses */

        if (Time.time - LastLandTime < 1F)
            LastJumpTilt = (LastJumpTilt + 1) % 2;
        else
            LastJumpTilt = 0F;

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
        Vector3 Move = CameraView.rotation * new Vector3(Local[0], 0F, Local[1]);

        Move[1] = 0F;
        Move.Normalize();

        Vector3 Velocity = Actor.velocity;

        HoldingJump &= PlayerInput.GetXButton;

        float gravitational_pull = PlayerVars.GRAVITY;
        float YComp = Velocity[1];
        float percent = YComp / InitialSpeed;

        /* Continual Ledge Detection  */
        if (/* only climb upward if falling */ 
            VectorHeader.Dot(Machine.GetActor.velocity, Vector3.up) <= MaxLedgeVelocity &&
            LedgeRegistry.DetectLedge(LedgeRegistry.GetProbeDistance,
            Actor._position,
            Machine.GetModelView.forward,
            Actor.orientation,
            out Vector3 ledge_position))
        {
            Machine.GetFSM.SwitchState(
                (ActorState next) =>
                {
                    ((LedgeState)next).Prepare(Actor._position, ledge_position);
                },
                "Ledge");

            /* attach callback to process setting our initial values on time */
            return;
        }

        if (HoldingJump)
            gravitational_pull = GravityCurve.Evaluate(percent) * PlayerVars.GRAVITY;

        Velocity -= Vector3.up * gravitational_pull * fdt;

        /* Rotate Towards */
        if (Move.sqrMagnitude > 0F)
        {
            float Turn = TurnTimeCurve.Evaluate(percent);

            ModelView.rotation = Quaternion.RotateTowards(
                ModelView.rotation,
                Quaternion.LookRotation(Move, Vector3.up),
                Turn * MaxRotationSpeed * fdt);

            Vector3 HorizontalV = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));

            Velocity -= HorizontalV;
            HorizontalV += Move * (MaxMoveInfluence * Turn * fdt);
            HorizontalV = Vector3.ClampMagnitude(HorizontalV, MaxHorizontalSpeed);
            Velocity += HorizontalV;
        }

        Actor.SetVelocity(Velocity);
        Machine.GetAnimator.SetFloat("Time", FallTimeCurve.Evaluate(percent));
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {

    }
    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {
        /* Ground Transition */
        if (Machine.ValidGroundTransition(trace.normal, trace.collider))
        {
            Machine.GetFSM.SwitchState(
                (next) =>
                {
                    Machine.GetAnimator.SetTrigger("Land");
                }, "Ground");
            return;
        }
        else
        {
            /* We've struck a wall, we need to determine whether or not its safe to climb or not? */

            float XDeviation = Vector3.Angle(Vector3.up, trace.normal);
            XDeviation = Mathf.Abs(90F - XDeviation); // get angular dif

            if (XDeviation <= 5.0F)
            {
                Machine.GetFSM.SwitchState(
                    (ActorState next) =>
                    {
                        ((WallSlideState)next).Prepare(trace.normal, velocity);
                    }, "WallSlide");
            }
            else // not a wall 
            {

            }

            return;
        }
    }

}
