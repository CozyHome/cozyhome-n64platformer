using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class DiveState : ActorState
{
    [SerializeField] private AnimationCurve DiveTurnCurve;
    [SerializeField] private AnimationCurve DiveDeaccelerationCurve;
    [SerializeField] private int DivesPerAerialState = 1;
    [SerializeField] private float DiveHeight = 1.5F;
    [SerializeField] private float DiveSpeed = 26F;
    [SerializeField] private float DiveTurnSpeed = 180F;

    private float InitialSpeed = 0F;
    private int DiveCount = 0;

    public bool CheckDiveEligiblity() => DiveCount > 0;

    public override void Enter(ActorState prev)
    {
        Transform ModelView = Machine.GetModelView;
        ActorHeader.Actor Actor = Machine.GetActor;
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        Animator Animator = Machine.GetAnimator;
        Vector3 Velocity = Actor.velocity;

        LedgeRegistry.SetProbeDistance(LedgeRegistry.DIVE_DISTANCE);

        Animator.SetTrigger("Dive"); // goto dive
        Animator.SetInteger("Step", 0); // set step to zero
        Animator.SetFloat("Time", 0); // set time to zero

        /* clear velocity */
        for (int i = 0; i < 3; i++)
            Velocity[i] = 0F;

        /* grab forward direction of our character and use as influence vector */
        Velocity = (ModelView.forward * DiveSpeed) + (Vector3.up * Mathf.Sqrt(2F * PlayerVars.GRAVITY * DiveHeight));
        InitialSpeed = Velocity[1];

        DiveCount--;

        Actor.SetVelocity(Velocity);
        Actor.SetSnapEnabled(false);
    }

    public override void Exit(ActorState next)
    {
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        LedgeRegistry.SetProbeDistance(LedgeRegistry.REGULAR_DISTANCE);
    }

    public override void Tick(float fdt)
    {
        Animator Animator = Machine.GetAnimator;
        LedgeRegistry LedgeRegistry = Machine.GetLedgeRegistry;
        ActorHeader.Actor Actor = Machine.GetActor;
        Transform ModelView = Machine.GetModelView;
        Transform CameraView = Machine.GetCameraView;

        Vector3 Velocity = Actor.velocity;

        Vector2 Local = Machine.GetPlayerInput.GetRawMove;
        Vector3 Move = ActorStateHeader.ComputeMoveVector(Local, CameraView.rotation, Vector3.up);

        float YComp = Velocity[1];
        float percent = YComp / InitialSpeed;

        if (ActorStateHeader.Transitions.CheckGeneralLedgeTransition(Actor.position,
            ModelView.forward,
            Actor.orientation,
            LedgeRegistry,
            Machine))
            return;
        else if (Actor.Ground.stable && Mathf.Abs(VectorHeader.Dot(Velocity, Actor.Ground.normal)) <= 0.1F)
        {
            Actor.SetSnapEnabled(true);
            Machine.GetFSM.SwitchState("DiveSlide");
            return;
        }
        else
        {
            Vector3 HorizontalVelocity = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));
            Vector3 VerticalVelocity = Velocity - HorizontalVelocity;
            float len = HorizontalVelocity.magnitude;

            len -= DiveDeaccelerationCurve.Evaluate(len / DiveSpeed);

            HorizontalVelocity = ModelView.forward * len;

            ActorStateHeader.AccumulateConstantGravity(ref VerticalVelocity, fdt, PlayerVars.GRAVITY);

            ActorStateHeader.RepairTime(fdt,
                DiveTurnCurve.Evaluate(percent),
                DiveTurnSpeed,
                DiveSpeed,
                0F,
                Move,
                ModelView,
                ref Velocity);

            Animator.SetFloat("Speed", 1F - percent);
            Actor.SetVelocity(VerticalVelocity + HorizontalVelocity);
            return;
        }
    }

    protected override void OnStateInitialize()
    {
        Machine.GetActorEventRegistry.Event_ActorLanded += () =>
        {
            DiveCount = DivesPerAerialState;
        };
    }
    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity) { }
}
