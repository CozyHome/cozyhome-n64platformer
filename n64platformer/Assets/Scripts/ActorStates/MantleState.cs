using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class MantleState : ActorState
{
    [Header("Mantle Parameters")]
    [SerializeField] private float UpwardOffset;
    [SerializeField] private float InwardAcceleration;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve EaseCurve;
    [SerializeField] private AnimationCurve SpeedCurve;
    private float InitialUpwardSpeed;

    private Vector3 displacement;

    public void Prepare(Vector3 hang_position, Vector3 mantle_position) /* Called when player presses XButton in LedgeState */
    {
        displacement = mantle_position - hang_position;
    }

    protected override void OnStateInitialize()
    {

    }
    public override void Enter(ActorState prev)
    {
        ActorHeader.Actor Actor = Machine.GetActor;

        float dot = displacement[1];
        InitialUpwardSpeed = Mathf.Sqrt(2F * PlayerVars.GRAVITY * (dot + UpwardOffset));

        Actor.SetVelocity(Vector3.up * InitialUpwardSpeed);
        Actor.SetSnapEnabled(false);

        Machine.GetAnimator.SetInteger("Step", 1);
    }

    public override void Exit(ActorState next)
    {        
        ActorHeader.Actor Actor = Machine.GetActor;

        Actor.SetSnapEnabled(true);
        Machine.GetAnimator.SetInteger("Step", 0);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {
        if (VectorHeader.Dot(Machine.GetActor._velocity, ground.normal) < 0F)
        {
            Machine.GetAnimator.SetTrigger("Land");
            Machine.GetFSM.SwitchState("Ground");
        }
    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }

    public override void Tick(float fdt)
    {
        Transform ModelView = Machine.GetModelView;
        Animator Animator = Machine.GetAnimator;
        ActorHeader.Actor Actor = Machine.GetActor;
        Vector3 Velocity = Actor._velocity;

        /* grav */
        Velocity -= Vector3.up * (PlayerVars.GRAVITY * fdt);

        /* To give the player the ability to land on the platform in front of them, we'll apply an inward velocity every frame */

        float Percent = Velocity[1] / InitialUpwardSpeed;
        float Ease = EaseCurve.Evaluate(Percent);
        float Proportion = SpeedCurve.Evaluate(Percent);

        Velocity += ModelView.forward * (Proportion * InwardAcceleration * fdt);
        
        Animator.SetFloat("Time", Ease);
        Actor.SetVelocity(Velocity);
    }
}
