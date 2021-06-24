using com.cozyhome.Vectors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticOrbitState : CameraState
{
    [Header("References")]
    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private float MaxAutomaticSpeed = 80F;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve EasingCurve;

    [SerializeField] private float MaxTurnTime = 0.5F;

    private Quaternion ActorLastRotation;
    private float TurnTime, LastRotate;

    protected override void OnStateInitialize()
    {
        Machine.GetFSM.SetState(this);
    }

    public override void Enter(CameraState prev)
    {
        Machine.GetEventRegistry.Event_ActorTurn += OnActorTurn;
    }

    public override void Exit(CameraState next)
    {

        Machine.GetEventRegistry.Event_ActorTurn -= OnActorTurn;
        TurnTime = 0F;

        Machine.ApplyOrbitPosition();
    }

    public override void FixedTick(float fdt)
    {
        Quaternion CameraRotation = Machine.ViewRotation;

        bool LeftTrigger = PlayerInput.GetLeftTrigger;
        if (LeftTrigger)
        {
            Machine.GetFSM.SwitchState(
            (CameraState next) =>
            {
                ((AlignOrbitState)next).Prepare();
            }, "Align");
            return;
        }

        Vector2 Mouse = PlayerInput.GetRawMouse;
        if (Mouse.sqrMagnitude > 0F)
        {
            Machine.GetFSM.SwitchState("Manual");
            return;
        }

        // Get angular difference between our forward vector and actor's forward direction
        // if angular difference is greater than a certain threshold in both poles, 
        // rotate toward the forward dir. ezpz

        Vector2 Rotate = Vector2.zero;
        if (ActorLastRotation != Quaternion.identity)
        {
            Vector3 v1 = ActorLastRotation * Vector3.right;
            Vector3 v2 = CameraRotation * Vector3.right;
            
            // Get the larger sign..?
            float angle = Vector3.SignedAngle(v2,v1, Vector3.up);
            Debug.Log(angle);
            if (Mathf.Abs(angle) >= 178F)
                Rotate[0] = 0F;
            else
                Rotate[0] = angle;

            // if angle >= 175F, maybe make a new temporary state that does the following:
            // reconstruct our look rotation based on our displacement..?
            // instead have our look rotation constantly pointing toward the player..?

            // only problem is if we're continually altering our rotation here, the player
            // will continually base its movement coordinate plane on this..
            // this causes an infinite "spinning" effect that i'm trying to avoid entirely.
            // I think the temporary fix of having the player only rotate <= epsilon is a good temp
            // solution
        }

       // if (absAngle >= 175F)
       //     Rotate[0] = 0F;
        if (Mathf.Abs(Rotate[0]) > 15F)
        {
            TurnTime += fdt;
            TurnTime = Mathf.Min(TurnTime, MaxTurnTime);
        }
        else
        {
            TurnTime -= fdt;
            TurnTime = Mathf.Max(TurnTime, 0F);
        }

        float rate = EasingCurve.Evaluate(TurnTime / MaxTurnTime);
        rate *= (fdt); // remvoe fdt since rotate quaternion is already using fdt

        Machine.OrbitAroundTarget(Rotate * fdt); 
        Machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    {

    }

    private void OnActorTurn(Quaternion newRot) => ActorLastRotation = newRot;
}
