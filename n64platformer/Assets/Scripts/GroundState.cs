using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class GroundState : State
{
    // pre-initialized references
    [SerializeField] private Transform CameraView;
    [SerializeField] private Transform ModelView;
    [SerializeField] private Animator Animator;


    // values
    [SerializeField] private AnimationCurve AccelerationCurve; // how fast we accelerate based on speed
    [SerializeField] private AnimationCurve DeaccelerationCurve; // how fast we deaccelerate based on speed
    [SerializeField] private float MaxMoveSpeed = 8F;


    // initialized references
    private PlayerInput PlayerInput;
    private ActorHeader.Actor PlayerActor;

    protected override void OnStateInitialize()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();
        PlayerInput = GetComponent<PlayerInput>();

        machine.SetCurrentState(this.Key);
    }

    public override void Enter(string previous_key, State previous_state)
    {

    }

    public override void Exit(string next_key, State next_state)
    {

    }

    public override void Tick(float fdt)
    {
        Vector2 Local = PlayerInput.GetRawMove;
        Quaternion CameraRotation = CameraView.rotation;
        Vector3 Move = CameraRotation * new Vector3(Local[0], 0F, Local[1]);
        Vector3 Velocity = PlayerActor._velocity;
        float Speed = Velocity.magnitude;

        Move[1] = 0F;
        Move.Normalize();

        if (Move.sqrMagnitude > 0.05F) 
        {
            ModelView.rotation = Quaternion.RotateTowards(
                ModelView.rotation,
                Quaternion.LookRotation(Move, Vector3.up),
                480F * fdt);   
            
            Speed += AccelerationCurve.Evaluate(Speed / MaxMoveSpeed) * fdt * MaxMoveSpeed;
            Speed = Mathf.Min(Speed, MaxMoveSpeed);
        }
        else 
        {
            Speed -= DeaccelerationCurve.Evaluate(Speed / MaxMoveSpeed) * fdt * MaxMoveSpeed;
            Speed = Mathf.Max(Speed, 0F);
        }

        Velocity = ModelView.rotation * new Vector3(0, 0, 1F);
        PlayerActor.SetVelocity(Velocity * Speed);

        Animator.SetFloat("Speed", Speed / MaxMoveSpeed);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {

    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }

}
