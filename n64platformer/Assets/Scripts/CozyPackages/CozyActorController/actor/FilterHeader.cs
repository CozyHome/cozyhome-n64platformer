using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.cozyhome.Actors
{
    public static class FilterHeader
    {
        // Simply a copy of ArchetypeHeader.TraceFilters.FindClosestFilterInvalids() with added trigger functionality
        public static void ActorTraceFilter(
               ActorHeader.IActorReceiver receiver,
               ref int _tracesfound,
               out int _closestindex,
               float _bias,
               Collider _self,
               RaycastHit[] _hits)
        {
            int nb_found = _tracesfound;
            float _closestdistance = Mathf.Infinity;
            _closestindex = -1;

            for (int i = nb_found - 1; i >= 0; i--)
            {
                _hits[i].distance -= _bias;
                RaycastHit _hit = _hits[i];
                Collider _col = _hit.collider;
                float _tracelen = _hit.distance;
                bool filterout = false;

                // if our dist is less than zero OR our collider is ourselves
                if (_tracelen <= 0F || _col == _self)
                    filterout = true;

                // if we aren't already filtering ourselves out, check to see if we're a collider
                if (!filterout && _hit.collider.isTrigger)
                {
                    receiver.OnTriggerHit(ActorHeader.TriggerHitType.Traced, _col);
                    filterout = true;
                }

                if (filterout)
                {
                    nb_found--;

                    if (i < nb_found)
                        _hits[i] = _hits[nb_found];
                }
                else
                {
                    if (_tracelen < _closestdistance)
                    {
                        _closestdistance = _tracelen;
                        _closestindex = i;
                    }

                    continue;
                }
            }

            _tracesfound = nb_found;
        }

        // Simply a copy of ArchetypeHeader.OverlapFilters.FilterSelf() with trigger checking
        public static void ActorOverlapFilter(
                ActorHeader.IActorReceiver receiver,
                ref int _overlapsfound,
                Collider _self,
                Collider[] _colliders)
        {
            int nb_found = _overlapsfound;
            for (int i = nb_found - 1; i >= 0; i--)
            {
                bool filterout = false;
                Collider col = _colliders[i];
                if (col == _self) // if we are the actor's collider
                    filterout = true;

                // we only want to filter out triggers that aren't the actor. Having an imprecise implementation of this filter
                // may lead to unintended consequences for the end-user.
                if (!filterout && col.isTrigger)
                {
                    receiver.OnTriggerHit(ActorHeader.TriggerHitType.Overlapped, col); // invoke a callback to whoever is listening
                    filterout = true;
                }

                if (filterout)
                {
                    nb_found--;

                    if (i < nb_found)
                        _colliders[i] = _colliders[nb_found];
                }
                else
                    continue;
            }

            _overlapsfound = nb_found;
        }
    }
}