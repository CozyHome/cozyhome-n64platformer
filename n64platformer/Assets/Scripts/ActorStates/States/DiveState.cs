using com.cozyhome.Actors;
using UnityEngine;

public class DiveState : ActorState
{
    [SerializeField] private AnimationCurve DiveTurnCurve;
    [SerializeField] private int DivesPerAerialState = 1;
    [SerializeField] private float DiveHeight = 1.5F;
    [SerializeField] private float DiveSpeed = 26F;
    [SerializeField] private float DiveTurnSpeed = 180F;

    private float InitialSpeed = 0F;
    private int DiveCount = 0;

    public bool CheckDiveEligiblity() { return DiveCount > 0; }

    public override void Enter(ActorState prev)
    {
        Transform ModelView = Machine.GetModelView;
        ActorHeader.Actor Actor = Machine.GetActor;
        Animator Animator = Machine.GetAnimator;
        Vector3 Velocity = Actor.velocity;

        Animator.SetTrigger("Dive");

        /* clear velocity */
        for (int i = 0; i < 3; i++)
            Velocity[i] = 0F;

        /* grab forward direction of our character and use as influence vector */
        Velocity = (ModelView.forward * DiveSpeed) + (Vector3.up * Mathf.Sqrt(2F * PlayerVars.GRAVITY * DiveHeight));
        InitialSpeed = Velocity[1];
        DiveCount--;

        Actor.SetVelocity(Velocity);
    }

    public override void Exit(ActorState next)
    {

    }

    public override void Tick(float fdt)
    {
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
        else if (Actor.Ground.stable)
        {
            Machine.GetFSM.SwitchState("Ground");
            return;
        }
        else
        {
            Vector3 HorizontalVelocity = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));
            Vector3 VerticalVelocity = Velocity - HorizontalVelocity;
            float len = HorizontalVelocity.magnitude;

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

            Actor.SetVelocity(VerticalVelocity + HorizontalVelocity);
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
