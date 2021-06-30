using com.cozyhome.Actors;
using com.cozyhome.Systems;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class CharacterActor : MonoBehaviour, ActorHeader.IActorReceiver, IEntity
{
    [SerializeField] private BoxActor Actor;

    void Start()
    {
        // ActorSystem.Register(this);
    }

    public void StartFrame()
    {
        Actor.SetPosition(transform.position);
        Actor.SetOrientation(transform.rotation);
        Actor.SetVelocity(Actor.orientation * Vector3.forward * 10F);
    }

    /* At the moment, CharacterActor placement inside of the ActorSystem will determine who has authority in 
        pushing others. This isn't good if one is attempting to manage server-side movement. However, you can
        get away from this by simply storing your actors in a SortedList<> rather than a List and iterating that way.
        You can use player ID or Client IDs to arbitrarily choose who moves first. It's stupid, I know. I'm not an
        expert in resolving these types of cases...

        We need to send our actor data to the transform so later entities can find the correct positional data during
        their traces/overlaps
         */
    public void MoveFrame(float fdt)
    {
        ActorHeader.Move(this, Actor, fdt);
        transform.SetPositionAndRotation(Actor.position, Actor.orientation);
    }
    public void EndFrame() => transform.SetPositionAndRotation(Actor.position, Actor.orientation);

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public void OnTraceHit(ActorHeader.TraceHitType type, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }

    public void OnInsertion() { }
    public void OnRemoval() { }
}
