using UnityEngine;

using System.Collections.Generic;

using com.cozyhome.Systems;
using static com.cozyhome.Actors.ActorHeader;
using com.cozyhome.Actors;

public class ActorSystem : MonoBehaviour,
    ActorHeader.IActorReceiver,
    SystemsHeader.IDiscoverSystem, 
    SystemsHeader.IFixedSystem
{
    private List<Actor> actors;

    public void OnDiscover()
    {
        actors = new List<Actor>();

        SystemsInjector.RegisterFixedSystem( 20, this);
    }

    public void OnFixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        // for loop begin 
        for(int i = 0;i < actors.Count;i++)
        {
            // get transform component on gameobject
            // and assign its contents to actor
            
            Actor a     = actors[i];
            Transform t = a.transform;

            a.SetPosition(t.position);
            a.SetOrientation(t.rotation);

        }

        // for loop move
        for(int i = 0;i < actors.Count;i++)
        {
            // move
            ActorHeader.Move(this, actors[i], fdt);
        }

        // for loop end
    
        for(int i = 0;i < actors.Count;i++)
        {
            // get newly calculated position and rotation
            // assign to transform
            Actor a     = actors[i];
            Transform t = a.transform;

            t.SetPositionAndRotation(a.position, a.orientation);
        }
    }

    public void AddActor(Actor a) 
    {
        if(!actors.Contains(a))
            actors.Add(a);
        else 
            return;
    }

    public void RemoveActor(Actor a) 
    {
        if(actors.Contains(a))
            actors.Remove(a);
        else 
            return;
    }

    // useless for now
    public void OnGroundHit(GroundHit ground, GroundHit lastground, LayerMask layermask)
    {
    
    }

    public void OnTraceHit(TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity)
    {
    
    }

    public void OnTriggerHit(TriggerHitType triggertype, Collider trigger)
    {
    
    }
}