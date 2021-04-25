using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealignOrbitState : CameraState
{
    [SerializeField] private AnimationCurve RealignCurve;
    [SerializeField] private float MaxTurnTime = 1F;

    private Quaternion InitialRotation, FinalRotation;
    private float EnterTime;

    protected override void OnInitialize()
    { }
    
    public override void Enter(string previous_key, CameraState previous_state)
    {
        machine.ComputeRealignments(ref InitialRotation, ref FinalRotation);
        EnterTime = Time.time;
    }

    public override void Exit(string next_key, CameraState next_state)
    { 
        machine.ApplyOrbitPosition();
    }

    public override void FixedTick(float fdt)
    {   
        float et = (Time.time - EnterTime) / MaxTurnTime;
        if(et > 1.0F) 
        {
            machine.SwitchCurrentState("Automatic");
            return;
        }
    

        float rate = RealignCurve.Evaluate(et); 

        machine.SetViewRotation(
            Quaternion.Slerp(
                InitialRotation,
                FinalRotation,
                rate)
        );

        machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    { }
}
