using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignOrbitState : CameraState
{
    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve EasingCurve;
    [SerializeField] private float MaxTurnTime = 0.5F;
    private Quaternion Initial, Final;

    [SerializeField] private PlayerInput PlayerInput;

    private float TurnTime;
    
    protected override void OnStateInitialize() { }

    public void Prepare() => Machine.ComputeRealignments(ref Initial, ref Final);

    public override void Enter(CameraState prev) { }

    public override void Exit(CameraState next)
    {
        TurnTime = 0F;
        Machine.ApplyOrbitPosition();
    }

    public override void FixedTick(float fdt)
    {
    
        Vector2 Mouse = PlayerInput.GetRawMouse;

        if(Mouse.sqrMagnitude > 0.001F || TurnTime >= MaxTurnTime)
        {
            Machine.GetFSM.SwitchState("Automatic");
            return;
        }

        TurnTime += fdt;
        TurnTime = Mathf.Min(TurnTime, MaxTurnTime);

        float rate = EasingCurve.Evaluate(TurnTime / MaxTurnTime);

        Machine.SetViewRotation(
            Quaternion.Slerp(
                Initial,
                Final,
                rate)
        );

        Machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt) { }
}
