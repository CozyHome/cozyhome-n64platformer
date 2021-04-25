using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualOrbitState : CameraState
{
    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private float MaxTurnTime = 1F;
    [SerializeField] private float ManualSensitivity = 240F;

    [SerializeField] private AnimationCurve EaseCurve;
    private float TurnTime;

    protected override void OnInitialize() { }

    public override void Enter(string previous_key, CameraState previous_state)
    {

    }

    public override void Exit(string next_key, CameraState next_state)
    {
        machine.ApplyOrbitPosition();
        TurnTime = 0F;
    }

    public override void FixedTick(float fdt)
    {
        bool LeftTrigger = PlayerInput.GetLeftTrigger;
        if (LeftTrigger)
        {
            machine.SwitchCurrentState("Realign");
            return;
        }

        Vector2 Mouse = PlayerInput.GetRawMouse;

        if (Mouse.sqrMagnitude > 0F)
        {
            TurnTime += fdt;
            TurnTime = Mathf.Min(TurnTime, MaxTurnTime);
        }
        else
        {
            TurnTime -= fdt;
            TurnTime = Mathf.Max(TurnTime, 0F);
        }

        float rate = EaseCurve.Evaluate(TurnTime / MaxTurnTime);
        rate *= ManualSensitivity;
        rate *= fdt;

        machine.OrbitAroundTarget(Mouse * rate);
        machine.ApplyOrbitPosition();
        machine.ApplyOrbitPositionGround();
    }

    public override void Tick(float dt)
    {

    }
}
