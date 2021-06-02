using UnityEngine;

namespace com.cozyhome.Vectors
{
    public static class VectorHeader
    {
        public static float Dot(Vector2 _a, Vector2 _b)
        {
            float _d = 0;
            for (int i = 0; i < 2; i++)
                _d += _a[i] * _b[i];
            return _d;
        }

        public static Vector2 ProjectVector(Vector2 _v, Vector2 _n)
        {
            Vector2 _c = Vector2.zero;
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _c[i] = _n[i] * _d;

            return _c;
        }

        public static void ProjectVector(ref Vector2 _v, Vector2 _n)
        {
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _v[i] = _n[i] * _d;
        }

        public static Vector2 ClipVector(Vector2 _v, Vector2 _n)
        {
            Vector2 _c = Vector2.zero;
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _c[i] = _v[i] - _n[i] * _d;

            return _c;
        }

        public static void ClipVector(ref Vector2 _v, Vector2 _n)
        {
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _v[i] = _v[i] - _n[i] * _d;
        }

        public static float Dot(Vector3 _a, Vector3 _b)
        {
            float _d = 0;
            for (int i = 0; i < 3; i++)
                _d += _a[i] * _b[i];
            return _d;
        }

        public static Vector3 ProjectVector(Vector3 _v, Vector3 _n)
        {
            Vector3 _c = Vector3.zero;
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _c[i] = _n[i] * _d;
            return _c;
        }

        public static void ProjectVector(ref Vector3 _v, Vector3 _n)
        {
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _v[i] = _n[i] * _d;
        }

        public static Vector3 ClipVector(Vector3 _v, Vector3 _n)
        {
            Vector3 _c = Vector3.zero;
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _c[i] = _v[i] - _n[i] * _d;
            return _c;
        }

        public static void ClipVector(ref Vector3 _v, Vector3 _n)
        {
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _v[i] = _v[i] - _n[i] * _d;
        }

        public static Vector3 ClosestPointOnPlane(
            Vector3 _point,
            Vector3 _planecenter,
            Vector3 _planenormal)
        => _point - ProjectVector(_planecenter - _point, _planenormal);

        public static Vector3 CrossProjection(
            Vector3 _v,
            Vector3 _u,
            Vector3 _n)
        {
            float _m = _v.magnitude;
            Vector3 _r = Vector3.Cross(_v, _u);
            _v = Vector3.Cross(_n, _r);
            _v.Normalize();
            return _v * _m;
        }

        public static void CrossProjection(
            ref Vector3 _v,
            Vector3 _u,
            Vector3 _n)
        {
            float _m = _v.magnitude;
            Vector3 _r = Vector3.Cross(_v, _u);
            Vector3 _f = Vector3.Cross(_n, _r);
            if (_f.sqrMagnitude > 0)
            {
                _v = _f;
                _v.Normalize();
                _v *= _m;
            }
        }
    }
}