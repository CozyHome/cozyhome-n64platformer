using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public class GroundState : State
{
    [SerializeField] private Transform CameraView;
    [SerializeField] private Transform ModelView;
    [SerializeField] private Animator Animator;

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
        Move[1] = 0F;
        Move.Normalize();

        if (Move.sqrMagnitude > 0F)
            ModelView.rotation = Quaternion.RotateTowards(
                ModelView.rotation,
                Quaternion.LookRotation(Move, Vector3.up),
                480F * fdt);

        PlayerActor.SetVelocity(Move);

        Animator.SetFloat("Speed", PlayerActor._velocity.magnitude / 8F);
    }

    public override void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {

    }

    public override void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }

}
