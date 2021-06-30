using UnityEngine;
using com.cozyhome.Actors;

namespace com.cozyhome.Debugging
{
    public class SimpleFPSMover : MonoBehaviour, ActorHeader.IActorReceiver
    {
        [Header("FPS Mover Components")]
        [SerializeField] private Transform _View;
        [SerializeField] private ActorHeader.Actor _Actor;

        [Header("Movement Parameters")]
        [SerializeField] [Range(1F, 720F)] private float _LookSensitivity = 360F;
        [SerializeField] [Range(0, 89.9F)] private float _MaxVerticalViewAngle = 85F;
        [SerializeField] [Range(0, 100F)] private float _MaxMovementSpeed = 12F;

        private void FixedUpdate()
        {
            if (!_Actor)
                return;
            else
            {
                _Actor.position = transform.position;

                Vector2 _mouse = new Vector2(
                    Input.GetAxisRaw("Mouse X"),
                    Input.GetAxisRaw("Mouse Y")
                );

                Quaternion R = LookRotate(
                    _View.rotation,
                    _mouse,
                    _MaxVerticalViewAngle
                );

                _View.rotation = R;

                Cursor.lockState = CursorLockMode.Locked;

                Vector2 _input =
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                Vector3 _wishvel = _View.rotation * new Vector3(_input[0], 0, _input[1]);
                _wishvel = Vector3.ClampMagnitude(_wishvel, 1.0F);

                if (_Actor.Ground.stable)
                {
                    Vector3 _rit = Vector3.Cross(_wishvel, _Actor.orientation * new Vector3(0, 1, 0));
                    _rit.Normalize();

                    Vector3 _fwd = Vector3.Cross(_Actor.Ground.normal, _rit);
                    _fwd.Normalize();

                    _wishvel = _fwd * (_wishvel.magnitude);

                    _Actor.SetVelocity(_wishvel * _MaxMovementSpeed);
                }
                else
                    _Actor.SetVelocity(_Actor.velocity - Vector3.up * Time.fixedDeltaTime * 39.62F);

                if (_Actor.Ground.stable && Input.GetAxis("Fire1") > 0)
                {
                    _Actor.SetSnapEnabled(false);
                    _Actor.SetVelocity(_Actor.velocity + Vector3.up * 10F);
                }

                ActorHeader.Move(this, _Actor, Time.fixedDeltaTime);

                transform.position = _Actor.position;
            }
        }

        private Quaternion LookRotate(
            Quaternion _previous,
            Vector2 _lookdelta,
            float _maxvertical)
        {
            Quaternion R = _previous;

            // measure the angular difference and adjust to clamped angle if need be:
            float _px = 90F - Vector3.Angle(
                _previous * new Vector3(0, 0, 1),
                new Vector3(0, 1, 0)
            );

            _lookdelta[0] *= (_LookSensitivity * Time.fixedDeltaTime);
            _lookdelta[1] *= (_LookSensitivity * Time.fixedDeltaTime);

            Vector3 fwd = R * new Vector3(0, 0, 1);

            float _nextx = -_lookdelta[1];

            // if (cur angle + delta angle) > clamp angle
            // subtract difference from delta and apply
            if (_px - _nextx > _maxvertical)
                _nextx = -(_maxvertical - _px);

            // do the same for the opposite axis
            else if (_px - _nextx < -_maxvertical)
                _nextx = -(-_maxvertical - _px);

            fwd = Quaternion.AngleAxis(
                    _nextx,
                    R * new Vector3(1, 0, 0)
                ) * fwd;

            fwd = Quaternion.AngleAxis(
                _lookdelta[0],
                new Vector3(0, 1, 0)
            ) * fwd;

            R = Quaternion.LookRotation(fwd);

            return R;
        }

        private Vector3 DetermineWishDirection()
        {
            return Vector3.zero;
        }

        public void OnGroundHit(ActorHeader.GroundHit _ground,
            ActorHeader.GroundHit _lastground,
            LayerMask _gfilter)
        { }

        public void OnTraceHit(ActorHeader.TraceHitType type, RaycastHit _trace, Vector3 _position, Vector3 _velocity)
        {
            bool _stbl = _Actor.DeterminePlaneStability(_trace.normal, _trace.collider);

            if (_stbl)
                _Actor.SetSnapEnabled(true);
        }

        public void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger)
        {
            Debug.Log(trigger.name);
        }
    }

}