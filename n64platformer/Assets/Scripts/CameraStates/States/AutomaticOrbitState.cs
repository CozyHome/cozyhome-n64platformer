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
    private float TurnTime;

    protected override void OnStateInitialize()
    {
        Machine.GetFSM.SetState(this);
    }

    public override void Enter(CameraState prev)
    {

    }

    public override void Exit(CameraState next)
    {
        TurnTime = 0F;
        Machine.ApplyOrbitPosition();
    }

    public override void FixedTick(float fdt)
    {
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

        Vector2 Move = PlayerInput.GetRawMove;
        Move[1] = 0F;

        if (Move.sqrMagnitude > 0F)
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
        rate *= (MaxAutomaticSpeed * fdt);

        Machine.OrbitAroundTarget(Move * rate);
        Machine.ApplyOrbitPosition();

    }

    public override void Tick(float dt)
    {

    }
}
