using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualOrbitState : CameraState
{

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve EasingCurve;

    [SerializeField] private float MaxEaseTime = 0.5F;
    private float EaseTime = 0F;


    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private float MaxOrbitSpeed = 240F;

    protected override void OnStateInitialize()
    {

    }

    public override void Enter(CameraState prev)
    {

    }

    public override void Exit(CameraState next)
    {
        EaseTime = 0F;
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
                ((AlignOrbitState) next).Prepare();
            }, "Align");
            return;
        }

        Vector2 Mouse = PlayerInput.GetRawMouse;

        if (Mouse.sqrMagnitude > 0F)
        {
            EaseTime += fdt;
            EaseTime = Mathf.Min(EaseTime, MaxEaseTime);
        }
        else
        {

            EaseTime -= fdt;
            EaseTime = Mathf.Max(EaseTime, 0F);
        }

        float rate = EasingCurve.Evaluate(EaseTime / MaxEaseTime);
        rate *= (MaxOrbitSpeed * fdt);

        Machine.OrbitAroundTarget(Mouse * rate);
        Machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    {

    }

}
