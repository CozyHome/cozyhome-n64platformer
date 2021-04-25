using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignOrbitState : CameraState
{
    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve EasingCurve;
    [SerializeField] private float MaxTurnTime = 0.5F;
    private Quaternion Initial, Final;

    private float TurnTime;

    public override void Enter(CameraState prev)
    {
        machine.ComputeRealignments(ref Initial, ref Final);
    }

    public override void Exit(CameraState next)
    {
        TurnTime = 0F;
        machine.ApplyOrbitPosition();
    }

    public override void FixedTick(float fdt)
    {
        if(TurnTime >= MaxTurnTime)
        {
            machine.GetFSM.SwitchState("Automatic");
            return;
        }

        TurnTime += fdt;
        TurnTime = Mathf.Min(TurnTime, MaxTurnTime);

        float rate = EasingCurve.Evaluate(TurnTime / MaxTurnTime);

        machine.SetViewRotation(
            Quaternion.Slerp(
                Initial,
                Final,
                rate
            )
        );

        machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    {

    }

    protected override void OnStateInitialize()
    {
    }

}
