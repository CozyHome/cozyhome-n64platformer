using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

namespace com.cozyhome.Archetype
{
    public static class ArchetypeHeader
    {
        public static int TraceRay(
            Vector3 _position,
            Vector3 _direction,
            float _mag,
            RaycastHit[] _traces,
            LayerMask _filter)
        {
            _position -= _direction * GET_TRACEBIAS(ARCHETYPE_LINE);

            int nbhits = Physics.RaycastNonAlloc(_position,
                _direction,
                _traces, _mag + GET_TRACEBIAS(ARCHETYPE_LINE),
                _filter,
                QueryTriggerInteraction.Ignore);

            return nbhits;
        }

        public static class OverlapFilters
        {
            public static void FilterSelf(
                ref int _overlapsfound,
                Collider _self,
                Collider[] _colliders)
            {
                int nb_found = _overlapsfound;
                for (int i = nb_found - 1; i >= 0; i--)
                {
                    if (_colliders[i] == _self)
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

        public static class TraceFilters
        {
            public static void FindClosestFilterInvalids(
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
                    float _tracelen = _hit.distance;

                    if (_tracelen > 0F &&
                        !_hit.collider.Equals(_self))
                    {
                        if (_tracelen < _closestdistance)
                        {
                            _closestdistance = _tracelen;
                            _closestindex = i;
                        }
                    }
                    else
                    {
                        nb_found--;

                        if (i < nb_found)
                            _hits[i] = _hits[nb_found];
                    }
                }
            }

            public static void FindClosestFilterInvalidsList(
                ref int _tracesfound,
                out int _closestindex,
                float _bias,
                List<Collider> _invalids,
                RaycastHit[] _hits)
            {
                int nb_found = _tracesfound;
                float _closestdistance = Mathf.Infinity;
                _closestindex = -1;

                for (int i = nb_found - 1; i >= 0; i--)
                {
                    _hits[i].distance -= _bias;
                    RaycastHit _hit = _hits[i];
                    float _tracelen = _hit.distance;

                    if (_tracelen > 0F && !_invalids.Contains(_hit.collider))
                    {
                        if (_tracelen < _closestdistance)
                        {
                            _closestdistance = _tracelen;
                            _closestindex = i;
                        }
                    }
                    else
                    {
                        nb_found--;

                        if (i < nb_found)
                            _hits[i] = _hits[nb_found];
                    }
                }
            }
        }

        /* Archetype Mappings */

        public static readonly int ARCHETYPE_SPHERE = 0;
        public static readonly int ARCHETYPE_CAPSULE = 1;
        public static readonly int ARCHETYPE_BOX = 2;
        public static readonly int ARCHETYPE_LINE = 3;

        private static readonly float[] SKINEPSILON = new float[3]
        {
            0.002F, // sphere
            0.002F, // capsule
            0.02F // box
        };

        private static readonly float[] TRACEBIAS = new float[4]
        {
            0.0002F, // sphere
            0.0002F, // capsule
            0.01F, // box
            0.0F // line
        };

        public static float GET_SKINEPSILON(int _i0) => SKINEPSILON[_i0];
        public static float GET_TRACEBIAS(int _i0) => TRACEBIAS[_i0];

        [System.Serializable]
        public abstract class Archetype
        {
            public abstract void Overlap(
                Vector3 _pos,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                Collider[] _colliders,
                out int _overlapcount);
            public abstract void Trace(
                Vector3 _pos,
                Vector3 _direction,
                float _len,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                RaycastHit[] _hits,
                out int _tracecount);

            public abstract Collider Collider();
            public abstract int PrimitiveType();

            public abstract void Inflate(float _amt);
            public abstract void Deflate(float _amt);

            public abstract Vector3 Center();
            public abstract float Height();

            public abstract Vector3 MaximizeConvexBoundary((Quaternion orient, Vector3 pos, Vector3 lc) cs, (Vector3 n, Vector3 x) pln);
            public abstract Vector3 ClosestPoint(Vector3 p);
        }

        [System.Serializable]
        public class SphereArchetype : Archetype
        {
            [SerializeField] SphereCollider _collider;

            public SphereArchetype(SphereCollider _collider)
            => this._collider = _collider;

            public override void Overlap(Vector3 _pos, Quaternion _orient, LayerMask _filter, float _inflate, QueryTriggerInteraction _interacttype, Collider[] _colliders, out int _overlapcount)
            {
                _overlapcount = 0;
                _pos += _orient * _collider.center;

                _overlapcount = Physics.OverlapSphereNonAlloc(
                    _pos,
                    _collider.radius + _inflate,
                    _colliders,
                    _filter,
                    _interacttype);
                return;
            }

            public override void Trace(Vector3 _pos, Vector3 _direction, float _len, Quaternion _orient, LayerMask _filter, float _inflate, QueryTriggerInteraction _interacttype, RaycastHit[] _hits, out int _tracecount)
            {
                _tracecount = 0;
                _pos += _orient * _collider.center;
                _pos -= _direction * TRACEBIAS[ARCHETYPE_SPHERE];

                _tracecount = Physics.SphereCastNonAlloc(
                    _pos,
                    _collider.radius + _inflate,
                    _direction,
                    _hits,
                    _len + TRACEBIAS[ARCHETYPE_SPHERE],
                    _filter,
                    _interacttype);
                return;
            }

            public override Collider Collider()
            => _collider;

            public override int PrimitiveType()
            => ARCHETYPE_SPHERE;

            public override void Inflate(float _amt)
            {
                _collider.radius += _amt;
            }

            public override void Deflate(float _amt)
            {
                _collider.radius -= _amt;
            }

            public static void Overlap(Vector3 _pos,
                Vector3 center,
                float radius,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                Collider[] _colliders,
                out int _overlapcount)
            {
                _overlapcount = 0;
                _pos += _orient * center;

                _overlapcount = Physics.OverlapSphereNonAlloc(
                    _pos,
                    radius + _inflate,
                    _colliders,
                    _filter,
                    _interacttype);
                return;
            }

            public static void Trace(Vector3 _pos,
                Vector3 center,
                Vector3 _direction,
                float _radius,
                float _len,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                RaycastHit[] _hits,
                out int _tracecount)
            {
                _tracecount = 0;
                _pos += _orient * center;
                _pos -= _direction * TRACEBIAS[ARCHETYPE_SPHERE];

                _tracecount = Physics.SphereCastNonAlloc(
                    _pos,
                    _radius + _inflate,
                    _direction,
                    _hits,
                    _len + TRACEBIAS[ARCHETYPE_SPHERE],
                    _filter,
                    _interacttype);
                
                return;
            }

            public override Vector3 Center() => _collider.center;
            public override float Height() => _collider.radius;

            public override Vector3 MaximizeConvexBoundary((Quaternion orient, Vector3 pos, Vector3 lc) cs, (Vector3 n, Vector3 x) pln)
                => pln.x + pln.n * _collider.radius;

            public override Vector3 ClosestPoint(Vector3 p) =>
                _collider.ClosestPoint(p);
        }

        [System.Serializable]
        public class CapsuleArchetype : Archetype
        {
            [SerializeField] CapsuleCollider _collider;

            public CapsuleArchetype(CapsuleCollider _collider)
            => this._collider = _collider;

            public override void Overlap(Vector3 _pos, Quaternion _orient, LayerMask _filter, float _inflate, QueryTriggerInteraction _interacttype, Collider[] _colliders, out int _overlapcount)
            {
                _overlapcount = 0;
                _pos += _orient * _collider.center;
                Vector3 _u = _orient * new Vector3(0, 1, 0);
                float rh = _inflate + _collider.height * .5F - _collider.radius;
                Vector3 _p0 = _pos - _u * (rh);
                Vector3 _p1 = _pos + _u * (rh);

                _overlapcount = Physics.OverlapCapsuleNonAlloc(_p0, _p1, _collider.radius + _inflate, _colliders, _filter, _interacttype);
                return;
            }
            public override void Trace(Vector3 _pos, Vector3 _direction, float _len, Quaternion _orient, LayerMask _filter, float _inflate, QueryTriggerInteraction _interacttype, RaycastHit[] _hits, out int _tracecount)
            {
                _tracecount = 0;
                _pos += _orient * _collider.center;
                _pos -= _direction * TRACEBIAS[ARCHETYPE_CAPSULE];

                Vector3 _u = _orient * new Vector3(0, 1, 0);
                float rh = _inflate + _collider.height * .5F - _collider.radius;
                Vector3 _p0 = _pos - _u * (rh);
                Vector3 _p1 = _pos + _u * (rh);

                _tracecount = Physics.CapsuleCastNonAlloc(
                    _p0,
                     _p1,
                     _collider.radius + _inflate,
                     _direction,
                     _hits,
                     _len + TRACEBIAS[ARCHETYPE_CAPSULE],
                     _filter,
                     _interacttype);
                return;
            }
            public override Collider Collider()
            => _collider;
            public override int PrimitiveType()
            => ARCHETYPE_CAPSULE;

            public override void Inflate(float _amt)
            {
                _collider.height += _amt;
                _collider.radius += _amt / 2F;
            }

            public override void Deflate(float _amt)
            {
                _collider.height -= _amt;
                _collider.radius -= _amt / 2F;
            }

            public override float Height() => _collider.height;

            public static void Overlap(Vector3 _pos,
                Vector3 _center,
                float _radius,
                float _height,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                Collider[] _colliders,
                out int _overlapcount)
            {
                _overlapcount = 0;
                _pos += _orient * _center;
                Vector3 _u = _orient * new Vector3(0, 1, 0);
                float rh = _inflate + _height * .5F - _radius;
                Vector3 _p0 = _pos - _u * (rh);
                Vector3 _p1 = _pos + _u * (rh);

                _overlapcount = Physics.OverlapCapsuleNonAlloc(_p0,
                                                        _p1,
                                                        _radius + _inflate,
                                                        _colliders,
                                                        _filter,
                                                        _interacttype);
                return;
            }

            public static void Trace(Vector3 _pos,
                Vector3 _center,
                float _height,
                float _radius,
                Vector3 _direction,
                float _len,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                RaycastHit[] _hits,
                out int _tracecount)
            {
                _tracecount = 0;
                _pos += _orient * _center;
                _pos -= _direction * TRACEBIAS[ARCHETYPE_CAPSULE];

                Vector3 _u = _orient * new Vector3(0, 1, 0);
                float rh = _inflate + _height * .5F - _radius;
                Vector3 _p0 = _pos - _u * (rh);
                Vector3 _p1 = _pos + _u * (rh);

                _tracecount = Physics.CapsuleCastNonAlloc(
                    _p0,
                     _p1,
                     _radius + _inflate,
                     _direction,
                     _hits,
                     _len + TRACEBIAS[ARCHETYPE_CAPSULE],
                     _filter,
                     _interacttype);
                return;
            }

            public override Vector3 Center() => _collider.center;

            public override Vector3 MaximizeConvexBoundary((Quaternion orient, Vector3 pos, Vector3 lc) cs, (Vector3 n, Vector3 x) pln)
            {
                Vector3 _u = cs.orient * new Vector3(0, 1, 0);
                cs.pos += cs.orient * cs.lc; // account for world-space transform
                
                float rh = _collider.height * .5F - _collider.radius;
                Vector3 _p0 = cs.pos - _u * (rh);
                Vector3 _p1 = cs.pos + _u * (rh);

                float a = Vector3.Dot(_p0 - pln.x, pln.n);
                float b = Vector3.Dot(_p1 - pln.x, pln.n);

                return a > b ? _p0 : _p1;
            }

            public override Vector3 ClosestPoint(Vector3 p) =>
                _collider.ClosestPoint(p);
        }

        [System.Serializable]
        public class BoxArchetype : Archetype
        {
            [SerializeField] BoxCollider _collider;

            public BoxArchetype(BoxCollider _collider)
            => this._collider = _collider;

            public override void Overlap(Vector3 _pos, Quaternion _orient, LayerMask _filter, float _inflate, QueryTriggerInteraction _interacttype, Collider[] _colliders, out int _overlapcount)
            {
                _overlapcount = 0;
                _pos += _orient * _collider.center;
                Vector3 _he = _collider.size * .5F;

                // inflate
                for (int i = 0; i < 3; i++)
                    _he[i] += _inflate;

                _overlapcount = Physics.OverlapBoxNonAlloc(_pos, _he, _colliders, _orient, _filter, _interacttype);
                return;
            }

            public override void Trace(Vector3 _pos, 
                Vector3 _direction, 
                float _len, 
                Quaternion _orient, 
                LayerMask _filter, 
                float _inflate, 
                QueryTriggerInteraction _interacttype, 
                RaycastHit[] _hits, 
                out int _tracecount)
            {
                _tracecount = 0;
                _pos += _orient * _collider.center;
                _pos -= _direction * TRACEBIAS[ARCHETYPE_BOX];

                Vector3 _he = _collider.size * .5F;
                for (int i = 0; i < 3; i++)
                    _he[i] += _inflate;

                _tracecount = Physics.BoxCastNonAlloc(_pos,
                _he,
                _direction,
                _hits,
                _orient,
                _len + TRACEBIAS[ARCHETYPE_BOX],
                _filter,
                _interacttype);
                return;
            }

            public override Collider Collider()
            => _collider;

            public override int PrimitiveType()
            => ARCHETYPE_BOX;

            public override void Inflate(float _amt)
            {
                Vector3 _sz = _collider.size;
                for (int i = 0; i < 3; i++)
                    _sz[i] += _amt;

                _collider.size = _sz;
            }

            public override void Deflate(float _amt)
            {
                Vector3 _sz = _collider.size;
                for (int i = 0; i < 3; i++)
                    _sz[i] -= _amt;

                _collider.size = _sz;
            }


            public static void Overlap(Vector3 _pos,
                Vector3 _center,
                Vector3 _size,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                Collider[] _colliders,
                out int _overlapcount)
            {
                _overlapcount = 0;
                _pos += _orient * _center;
                Vector3 _he = _size * .5F;

                // inflate
                for (int i = 0; i < 3; i++)
                    _he[i] += _inflate;

                _overlapcount = Physics.OverlapBoxNonAlloc(_pos, _he, _colliders, _orient, _filter, _interacttype);
                return;
            }

            public static void Trace(Vector3 _pos,
                Vector3 _center,
                Vector3 _size,
                Vector3 _direction,
                float _len,
                Quaternion _orient,
                LayerMask _filter,
                float _inflate,
                QueryTriggerInteraction _interacttype,
                RaycastHit[] _hits,
                out int _tracecount)
            {
                _tracecount = 0;
                _pos += _orient * _center;
                _pos -= _direction * TRACEBIAS[ARCHETYPE_BOX];

                Vector3 _he = _size * .5F;
                for (int i = 0; i < 3; i++)
                    _he[i] += _inflate;

                _tracecount = Physics.BoxCastNonAlloc(_pos,
                _he,
                _direction,
                _hits,
                _orient,
                _len + TRACEBIAS[ARCHETYPE_BOX],
                _filter,
                _interacttype);
                return;
            }

            public override Vector3 Center() => _collider.center;
            
            public override float Height() => _collider.size[1];

            public override Vector3 MaximizeConvexBoundary((Quaternion orient, Vector3 pos, Vector3 lc) cs, (Vector3 n, Vector3 x) pln)
            {
                // basically need to reconstruct every point in our box systematically
                // TIP: im sure an inverse matrix of our plane into coordinates relative to our shape
                // would greatly improve the running time of our algorithm. Sadly, I am very lazy.
                cs.pos += cs.orient * cs.lc;
                
                float max     = 0F;
                Vector3 v_max = Vector3.zero;

                for(int i = 0;i < 8;i++)
                {
                    Vector3 p = this.GetLocalBoundaryPoint(i);
                    // transform point to world space
                    p = cs.orient * p; // transform p itself to world space
                    p = p + cs.pos; // add the local space offset
                    float d = VectorHeader.Dot(p - pln.x, pln.n); 

                    if(d > max)
                    {
                        max = d;
                        v_max = p;
                    }
                }

                return v_max;
            }

            public override Vector3 ClosestPoint(Vector3 p) =>
                _collider.ClosestPoint(p);

            // really bad but necessary to prevent having to store an array of vector3s for every single box
            public Vector3 GetLocalBoundaryPoint(int i) 
            {
                float l,w,h; l = _collider.size[0] / 2F; w = _collider.size[1] / 2F; h = _collider.size[2] / 2F;
                
                switch(i)
                {
                    case 0:
                        return new Vector3(-l, -w, -h);
                    case 1:
                        return new Vector3(-l, +w, -h);
                    case 2:
                        return new Vector3(+l, -w, -h);
                    case 3:
                        return new Vector3(+l, +w, -h);
                    case 4:
                        return new Vector3(-l, -w, +h);    
                    case 5:
                        return new Vector3(-l, +w, +h);    
                    case 6:
                        return new Vector3(+l, -w, +h);    
                    case 7:
                        return new Vector3(+l, +w, +h);    
                }

                return Vector3.zero;
            }
        }
    }
}
