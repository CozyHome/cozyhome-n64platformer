using com.cozyhome.Archetype;
using UnityEngine;
using com.cozyhome.Vectors;
namespace com.cozyhome.Actors
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class CapsuleActor : ActorHeader.Actor
    {
        [System.NonSerialized] private ArchetypeHeader.CapsuleArchetype CapsuleArchetype;
        
        protected override void InitializeSpecifics()
        {
            CapsuleArchetype = new ArchetypeHeader.CapsuleArchetype(
                GetComponent<CapsuleCollider>()
            );
        }

        public override ArchetypeHeader.Archetype GetArchetype()
        => CapsuleArchetype;

        public override bool DeterminePlaneStability(Vector3 _normal, Collider _other)
        {
            return base.DeterminePlaneStability(_normal, _other);
        }

        public override bool DetermineGroundStability(Vector3 _vel, RaycastHit _hit, LayerMask _gfilter)
        {
            return base.DeterminePlaneStability(_hit.normal, _hit.collider)
                && DetermineEdgeStability(_vel, _hit.point, _hit.normal, _gfilter);
        }

        // TODO: 
        // rework ledge detection: 
        //  1. simplify/abstract this further so there is less boilerplate inlined in method
        //  2. fix scenarios in which capsules will orient themselves along the ledge and launching themselves
        // into the air

        // i think determination should not include modification of any source data unless I specifically tell it to inside of my 
        // slide algo

        // bitflags are the key to returning information about the determination if anything
        // this method/chain of execution is very ugly at the moment, no plans to fix anytime soon as i'd rather focus on other things.
        // the system does what I want it to and that's really all that matters

        // feel free to tear it apart though if you'd like :)
        private bool DetermineEdgeStability(in Vector3 _vel,
            Vector3 _hitpoint,
            Vector3 _hitnormal,
            LayerMask _filter)
        {
            Vector3 _u = orientation * new Vector3(0, 1, 0);
            Vector3 _fp = VectorHeader.ClipVector(_hitnormal, _u);
            _fp.Normalize();

            const float _auxvertheight = 0.05F;
            const float _auxvertwidth = 0.01F;
            int _eflags = 0;

            Vector3 _auxvert = _u * _auxvertheight;
            Vector3 _auxhori = _fp * _auxvertwidth;

            int _outercount = ArchetypeHeader.TraceRay(
                _hitpoint + _auxvert + _auxhori,
                -_u,
                _auxvertheight + 0.1F,
                _internalhits,
                _filter);

            ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(ref _outercount,
                out int _o0,
                ArchetypeHeader.GET_TRACEBIAS(ArchetypeHeader.ARCHETYPE_LINE),
                null,
                _internalhits);

            if (_o0 >= 0 && this.DeterminePlaneStability(_internalhits[_o0].normal, _internalhits[_o0].collider))
                _eflags |= (1 << 0);

            int _innercount = ArchetypeHeader.TraceRay(
                _hitpoint + _auxvert - _auxhori,
                -_u,
                _auxvertheight + 0.1F,
                _internalhits,
                _filter);

            ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(ref _innercount,
                out int _i0,
                ArchetypeHeader.GET_TRACEBIAS(ArchetypeHeader.ARCHETYPE_LINE),
                null,
                _internalhits);

            if (_i0 >= 0 && this.DeterminePlaneStability(_internalhits[_i0].normal, _internalhits[_i0].collider))
                _eflags |= (1 << 1);

            float _ed = VectorHeader.Dot(velocity, _fp);

            if (_eflags != ((1 << 0) | (1 << 1))) // ledge detected & velocity exiting normal dir
            {
                if (_ed > 0F)
                    return false;
                else
                {
                    VectorHeader.ClipVector(ref velocity, _u);
                    return true;
                }
            }
            else
                return true;
        }
    }
}