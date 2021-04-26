using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleOrbitState : CameraState
{

    protected override void OnStateInitialize()
    {
        machine.GetEventRegistry.Event_ActorLanded += delegate 
        {
            machine.GetFSM.SwitchState("Automatic");  
        };
    }

    public override void Enter(CameraState prev)
    {

    }

    public override void Exit(CameraState next)
    {

    }

    public override void FixedTick(float fdt)
    {
        machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    {

    }
}
