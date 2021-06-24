using System;
using UnityEngine;

public class ActorEventRegistry : MonoBehaviour
{
    public Action Event_ActorJumped;
    public Action Event_ActorLanded;
    public Action<Vector3> Event_ActorFoundLedge;
    public Action<Quaternion> Event_ActorTurn;

#pragma warning disable IDE0051 // Remove unused private members
    void Start()
#pragma warning restore IDE0051 // Remove unused private members
    {
        Event_ActorJumped = null;
        Event_ActorLanded = null;
        Event_ActorFoundLedge = null;
    }
}
