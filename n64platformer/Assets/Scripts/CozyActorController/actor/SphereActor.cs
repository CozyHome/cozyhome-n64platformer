using com.cozyhome.Archetype;
using UnityEngine;

namespace com.cozyhome.Actors
{
    [RequireComponent(typeof(SphereCollider))] public class SphereActor : ActorHeader.Actor
    {
        [System.NonSerialized] private ArchetypeHeader.SphereArchetype SphereArchetype;
        /* UnityEngine */
        protected override void InitializeSpecifics() 
        {
            SphereArchetype = new ArchetypeHeader.SphereArchetype(
                GetComponent<SphereCollider>()
            );
        }

        public override ArchetypeHeader.Archetype GetArchetype()
        => SphereArchetype;

        public override bool DetermineGroundStability(Vector3 _vel, RaycastHit _hit, LayerMask _gfilter)
            => base.DeterminePlaneStability(_hit.normal, _hit.collider);
    }
}