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
            //VectorHeader.ClipVector(ref v2, Vector3.up);
            //v2.Normalize();

            // Get the larger sign..?
            float dot = Vector3.SignedAngle(v2,v1, Vector3.up);
            Rotate[0] = dot;
            Debug.Log(Rotate[0]);
            //dot += 1;
            //dot /= 2;
            //dot *= 90F;

            ActorLastRotation = Quaternion.identity;
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

        Machine.OrbitAroundTarget(Rotate * rate); 
        Machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    {

    }

    private void OnActorTurn(Quaternion newRot) => ActorLastRotation = newRot;
}
