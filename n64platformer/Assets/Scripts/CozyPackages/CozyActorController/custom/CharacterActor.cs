using com.cozyhome.Actors;
using com.cozyhome.Systems;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class CharacterActor : MonoBehaviour, ActorHeader.IActorReceiver, IEntity
{
    [SerializeField] private ActorHeader.Actor Actor;

    void Start()
    {
        // ActorSystem.Register(this);
    }

    public void StartFrame()
    {
        float fdt = Time.fixedDeltaTime;
        Actor.SetPosition(transform.position);
        Actor.SetOrientation(transform.rotation);
        Actor.SetVelocity(Actor.orientation * Vector3.forward * 20F);
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

    // We may want to keep track of our last fixed update transforms for systems like interpolating, etc.
    public void EndFrame() => transform.SetPositionAndRotation(Actor.position, Actor.orientation);

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask) { }
    public void OnTraceHit(ActorHeader.TraceHitType type, RaycastHit trace, Vector3 position, Vector3 velocity) { }
    public void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger) { }

    public void OnInsertion() { }
    public void OnRemoval() { }
}
