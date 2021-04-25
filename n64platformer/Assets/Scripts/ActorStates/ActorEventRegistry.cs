using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEventRegistry : MonoBehaviour
{
    public Action Event_ActorJumped;

    void Start() 
    {
        Event_ActorJumped = null;
    }
}
