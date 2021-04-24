using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class GroundState : State
{
    // pre-initialized references
    [SerializeField] private Transform CameraView;
    [SerializeField] private Transform ModelView;
    [SerializeField] private Animator Animator;
    [SerializeField] private PlayerInput PlayerInput;


    // values
    [SerializeField] private AnimationCurve AnimatorSpeedCurve; 
    [SerializeField] private AnimationCurve AccelerationCurve; // how fast we accelerate based on speed
    [SerializeField] private AnimationCurve DeaccelerationCurve; // how fast we deaccelerate based on speed
    [SerializeField] private float MaxMoveSpeed = 8F;
    [SerializeField] private Vector3 LastVelocity;


    // initialized references
    private ActorHeader.Actor PlayerActor;

    protected override void OnStateInitialize()
    {
        PlayerActor = GetComponent<ActorHeader.Actor>();
        machine.SetCurrentState(this.Key);
    }

    public override void Enter(string previous_key, State previous_state) { }

    public override void Exit(string next_key, State next_state) { }

    public override void Tick(float fdt)
    {
        Vector2 Local = PlayerInput.GetRawMove;
        Quaternion CameraRotation = CameraView.rotation;
        Vector3 Move = CameraRotation * new Vector3(Local[0], 0F, Local[1]);
        Vector3 Velocity = PlayerActor._velocity;
        float Speed = Velocity.magnitude;
        float Ratio = Speed / MaxMoveSpeed;

        Move[1] = 0F;
        Move.Normalize();

        if (Move.sqrMagnitude > 0.05F)
        {
            ModelView.rotation = Quaternion.RotateTowards(
                ModelView.rotation,
                Quaternion.LookRotation(Move, Vector3.up),
                960F * fdt);

            Speed += AccelerationCurve.Evaluate(Ratio) * fdt * MaxMoveSpeed;
            Speed = Mathf.Min(Speed, MaxMoveSpeed);
        }
        else
        {
            Speed -= DeaccelerationCurve.Evaluate(Ratio) * fdt * MaxMoveSpeed;
            Speed = Mathf.Max(Speed, 0F);
        }

        Velocity = ModelView.rotation * new Vector3(0, 0, 1F);

        if (PlayerActor.Ground.stable)
        {
            VectorHeader.CrossProjection(ref Velocity, Vector3.up, PlayerActor.Ground.normal);
        }

        PlayerActor.SetVelocity(Velocity * Speed);
        
        Animator.speed = AnimatorSpeedCurve.Evaluate(Ratio);
        Animator.SetFloat("Speed", Speed / MaxMoveSpeed);
    
        if(Input.GetKey(KeyCode.Space)) 
        {
            machine.SwitchCurrentState("Jump");
        }
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {

    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }

}
