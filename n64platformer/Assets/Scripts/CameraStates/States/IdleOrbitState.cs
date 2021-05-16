using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleOrbitState : CameraState
{

    protected override void OnStateInitialize()
    {
        /*
        Machine.GetEventRegistry.Event_ActorLanded += delegate 
        {
            if(Machine.GetFSM.Current.GetKey == this.GetKey)
                Machine.GetFSM.SwitchState("Automatic");  
        };
        */
    }

    public override void Enter(CameraState prev)
    {

    }

    public override void Exit(CameraState next)
    {

    }

    public override void FixedTick(float fdt)
    {
        Machine.ApplyOrbitPosition();
    }

    public override void Tick(float dt)
    {

    }
}
