using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEventRegistry : MonoBehaviour
{
    public Action Event_ActorJumped;
    public Action Event_ActorLanded;
    public Action<Vector3> Event_ActorFoundLedge;

    void Start() 
    {
        Event_ActorJumped = null;
        Event_ActorLanded = null;
        Event_ActorFoundLedge = null;
    }
}
