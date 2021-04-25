using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticOrbitState : CameraState
{
    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private float AutomaticSensitivity = 80F;
    [SerializeField] private float MaxTurnTime = 0.5F;
    [SerializeField] private AnimationCurve EasingCurve;
    private float TurnTime;

    protected override void OnInitialize()
    { 
        machine.SetCurrentState(this.GetKey);
    }

    public override void Enter(string previous_key, CameraState previous_state)
    {}

    public override void Exit(string next_key, CameraState next_state)
    {
        machine.ApplyOrbitPosition();
        TurnTime = 0F;
    }

    public override void FixedTick(float fdt)
    {        
        bool LeftTrigger = PlayerInput.GetLeftTrigger;
        if(LeftTrigger)
        {
            machine.SwitchCurrentState("Realign");
            return;
        }

        Vector2 Mouse = PlayerInput.GetRawMouse;
        if(Mouse.sqrMagnitude > 0F) 
        {
            machine.SwitchCurrentState("Manual");
            return;
        }

        Vector2 Move = PlayerInput.GetRawMove;
        Move[1] = 0F;

        if(Move.sqrMagnitude > 0F) 
        {
            TurnTime += fdt;
            TurnTime = Mathf.Min(TurnTime, MaxTurnTime);
        }
        else
        {
            TurnTime -= fdt;
            TurnTime = Mathf.Max(0F, TurnTime);
        }

        float rate = EasingCurve.Evaluate(TurnTime / MaxTurnTime);
        rate *= AutomaticSensitivity;
        rate *= fdt;

        machine.OrbitAroundTarget(Move * rate);
        machine.ApplyOrbitPosition();
        
    }   

    public override void Tick(float dt)
    { }

}
