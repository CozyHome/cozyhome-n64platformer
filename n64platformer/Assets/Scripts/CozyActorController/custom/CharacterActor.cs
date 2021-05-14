using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Actors;

public class CharacterActor : MonoBehaviour, ActorHeader.IActorReceiver
{
    [SerializeField] private BoxActor Actor;


    private void FixedUpdate() 
    {
        float fdt = Time.fixedDeltaTime;
        
        Actor.SetPosition(transform.position);
        Actor.SetOrientation(transform.rotation);
        Actor.SetVelocity(Vector3.forward * 10F);
    
        ActorHeader.Move(this, Actor, fdt);
    
        transform.SetPositionAndRotation(Actor.position, Actor.orientation);
    }

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    {

    }

    public void OnTraceHit(RaycastHit trace, Vector3 position, Vector3 velocity)
    {

    }
}
