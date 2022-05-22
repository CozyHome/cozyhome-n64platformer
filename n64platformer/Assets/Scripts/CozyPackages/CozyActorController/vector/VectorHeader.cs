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
        => _point + ProjectVector(_planecenter - _point, _planenormal);

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

        public static float LinePlaneIntersection((Vector3 p, Vector3 r) line, (Vector3 x, Vector3 n) plane) 
        {
            // (c - p) * n =  upper
            // (r) * n = lower
            float l = Dot(line.r, plane.n);
            if(Mathf.Approximately(l, 0F))
                return -1F;
            else
            {
                float u = Dot(plane.x - line.p, plane.n);
                return u / l;
            }
        }

        public static (Vector3 a, Vector3 b) ClosestPointTriangle(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            var a = VectorHeader.Barycentric2DClamped(tri, p);
            return (p ,tri.a * a[0] + tri.b * a[1] + tri.c * a[2]);
        }

        public static (Vector3 a, Vector3 b) ClosestPointEdge(
        (Vector3 a, Vector3 b) edge,
        Vector3 p) {
            
            Vector3 ao = p - edge.a;
            Vector3 bo = p - edge.b;
            Vector3 ab = edge.b - edge.a;
            Vector3 abo = Vector3.Cross(ao, bo);

            float am = ab.magnitude;
            ab /= am;

            float v = VectorHeader.Dot(p - edge.a, ab);
            
            if(v > am)
                return (p, edge.b);
            else if(v <= 0F)
                return (p, edge.a);
            else
                return (p, edge.a + ab * v);
        }

        public static int Barycentric1DVoronoi(
        (Vector3 a, Vector3 b) edge, (int a, int b) bits,
        Vector3 p) {
            
            Vector3 ao = p - edge.a;
            Vector3 bo = p - edge.b;
            Vector3 ab = edge.b - edge.a;
            Vector3 abo = Vector3.Cross(ao, bo);

            float am = ab.magnitude;
            ab /= am;

            float v = VectorHeader.Dot(p - edge.a, ab);
            
            if(v > am)
                return bits.b;
            else if(v <= 0F)
                return bits.a;
            else
                return bits.a | bits.b;
        }

        public static (int, Vector3) Barycentric1D_GJK(
        (Vector3 a, Vector3 b) edge, (int a, int b) bits,
        Vector3 p) {
            
            Vector3 ao = p - edge.a;
            Vector3 bo = p - edge.b;
            Vector3 ab = edge.b - edge.a;
            Vector3 abo = Vector3.Cross(ao, bo);

            float am = ab.magnitude;
            ab /= (am > 0) ? (am) : 1;

            float v = VectorHeader.Dot(p - edge.a, ab);
            
            if(v > am)
                return (bits.b, edge.b);
            else if(v <= 0F)
                return (bits.a, edge.a);
            else
                return (bits.a | bits.b, edge.a + VectorHeader.ProjectVector(p - edge.a, ab));
        }

        public static float Barycentric1DClamped(
        (Vector3 a, Vector3 b) edge,
        Vector3 p) {
            
            Vector3 ao = p - edge.a;
            Vector3 ab = edge.b - edge.a;
            float   am = ab.magnitude;
            ab /= (am > 0) ? am : 1; 

            // REFACTOR THIS, A SIMPLE PROJECTION WILL SUFFICE
            return Mathf.Clamp01(VectorHeader.Dot(p - edge.a, ab / am));
        }

        public static Vector3 Barycentric2D(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            Vector3 abc  = Vector3.Cross(tri.b - tri.a, tri.c - tri.b);
            float area = abc.magnitude;
            abc /= area;

            Vector3 v = new Vector3(
                Vector3.Dot(Vector3.Cross(p - tri.b, tri.c - p), abc) / area, // 0 -> ab x ao oab
                Vector3.Dot(Vector3.Cross(p - tri.c, tri.a - p), abc) / area, // 1 -> bc x bo obc
                0F
            );
            
            v[2] = 1 - v[0] - v[1];  // 2 -> ca x co oca
            return v;
            
        } 

        public static Vector3 Barycentric2DClamped(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            Vector3 abc  = Vector3.Cross(tri.c - tri.a, tri.b - tri.a);
            float area = abc.magnitude;
            abc /= area;

            Vector3 ab_n = Vector3.Cross(abc, tri.b - tri.a);
            Vector3 bc_n = Vector3.Cross(abc, tri.c - tri.b);
            Vector3 ca_n = Vector3.Cross(abc, tri.a - tri.c);
            
            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            Vector3 Bary(Vector3 p) {
                Vector3 v = new Vector3(
                Vector3.Dot(Vector3.Cross(p - tri.b, tri.c - p), abc) / area, // 0 -> ab x ao oab
                Vector3.Dot(Vector3.Cross(p - tri.c, tri.a - p), abc) / area, // 1 -> bc x bo obc
                0F);
                v[2] = 1 - v[0] - v[1];  // 2 -> ca x co oca
                return v;
            }

            int ComputeSignedAreas(Vector3 p) {
                int nflags = 0;
                nflags |= Same(p - tri.a, ab_n) ? (1 << 0) : 0; // AB
                nflags |= Same(p - tri.b, bc_n) ? (1 << 1) : 0; // AB
                nflags |= Same(p - tri.c, ca_n) ? (1 << 2) : 0; // AB
                return nflags;
            }

            Vector3 DualEdges((Vector3 x, Vector3 y, Vector3 z) pm) {
                // dual edges will serve as a permuted form of the triangle
                // xy, zy edges will be constructed for sign analysis
                Vector3 xy = pm.y - pm.x;
                Vector3 zy = pm.y - pm.z;

                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;
                Vector3 zp = p    - pm.z;

                Vector3 zxy = Vector3.Cross(zy, xy);

                Vector3 xy_n = Vector3.Cross(xy, zxy);
                Vector3 zy_n = Vector3.Cross(zxy, zy);
                
                if(Same(yp, zy) && Same(yp, xy))
                    return Bary(pm.y);

                if(!Same(yp, xy)) {
                    if(!Same(xp, xy))
                        return Bary(pm.x);
                    else
                        return Bary(p - VectorHeader.ProjectVector(yp, xy_n.normalized));
                }

                if(!Same(yp, zy)) {
                    if(!Same(zp, zy))
                        return Bary(pm.z);
                    else
                        return Bary(p - VectorHeader.ProjectVector(yp, zy_n.normalized));
                }

                return Vector3.zero;
            }

            Vector3 Edge((Vector3 x, Vector3 y) pm) {
                Vector3 xy = pm.y - pm.x;
                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;

                if(Same(xy, yp) && Same(xy, xp))
                    return Bary(pm.y);
                
                if(!Same(xy, yp) && !Same(xy, xp))
                    return Bary(pm.x);

                return Bary(pm.x + VectorHeader.ProjectVector(xp, xy.normalized));
            }

            // ALGORITHM
            // DETERMINE THE NUMBER OF POSITIVE SIGNED PROJECTIONS ALONG EDGE PLANES 
            // IF PLANE COUNT IS TWO: (NFLAGS, NCOUNT)
            // -----> DETERMINE ORIENTATION OF THE THREE EDGE VERTICES VIA BITS 
            //      ------> COMPUTE VORONOI REGION VIA THESE TWO EDGES
            // ELIF PLANE COUNT IS ONE: (NFLAGS, NCOUNT) (RUN BARYCENTRIC)
            // ------> DETERMINE ORIENTATION OF THE TWO EDGE VERTICES VIA BITS
            //      ------> COMPUTE VORONOI REGION VIA SINGULAR EDGE (RUN BARYCENTRIC)
            //
            //

            switch(ComputeSignedAreas(p)) { 
                case 1: // AB
                    return Edge((tri.a, tri.b));
                case 2: // BC
                    return Edge((tri.b, tri.c));
                case 3: // AB & BC (ABC)
                    return DualEdges((tri.a, tri.b, tri.c));
                case 4: // CA
                    return Edge((tri.c, tri.a));
                case 5: // CA & AB (CAB)
                    return DualEdges((tri.c, tri.a, tri.b));
                case 6: // CA & BC (BCA)
                    return DualEdges((tri.b, tri.c, tri.a));
                default:
                    return Bary(p);
            }
        }

        public static int Barycentric2DVoronoi(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {
            Vector3 abc  = Vector3.Cross(tri.c - tri.a, tri.b - tri.a);
            
            Vector3 ab_n = Vector3.Cross(abc, tri.b - tri.a);
            Vector3 bc_n = Vector3.Cross(abc, tri.c - tri.b);
            Vector3 ca_n = Vector3.Cross(abc, tri.a - tri.c);

            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            int ComputeSignedAreas(Vector3 p) {
                int nflags = 0;
                nflags |= Same(p - tri.a, ab_n) ? (1 << 0) : 0; // AB
                nflags |= Same(p - tri.b, bc_n) ? (1 << 1) : 0; // AB
                nflags |= Same(p - tri.c, ca_n) ? (1 << 2) : 0; // AB
                return nflags;
            }

            int DualEdges((Vector3 x, Vector3 y, Vector3 z) pm,
                          (int     x, int     y,     int z) bits) {
                // dual edges will serve as a permuted form of the triangle
                // xy, zy edges will be constructed for sign analysis
                Vector3 xy = pm.y - pm.x;
                Vector3 zy = pm.y - pm.z;

                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;
                Vector3 zp = p    - pm.z;

                Vector3 zxy = Vector3.Cross(zy, xy);

                Vector3 xy_n = Vector3.Cross(xy, zxy);
                Vector3 zy_n = Vector3.Cross(zxy, zy);
                
                if(Same(yp, zy) && Same(yp, xy))
                    return bits.y;

                if(!Same(yp, xy)) {
                    if(!Same(xp, xy))
                        return bits.x;
                    else
                        return bits.x | bits.y;
                }

                if(!Same(yp, zy)) {
                    if(!Same(zp, zy))
                        return bits.z;
                    else
                        return bits.y | bits.z;
                }

                return bits.x | bits.y | bits.z;
            }

            int Edge((Vector3 x, Vector3 y) pm, 
                     (int     x, int     y) bits) {
                Vector3 xy = pm.y - pm.x;
                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;

                if(Same(xy, yp) && Same(xy, xp))
                    return bits.y;
                
                if(!Same(xy, yp) && !Same(xy, xp))
                    return bits.x;

                return bits.x | bits.y;
            }

            switch(ComputeSignedAreas(p)) { 
                case 1: // AB
                    return Edge((tri.a, tri.b), (0x1, 0x2));
                case 2: // BC
                    return Edge((tri.b, tri.c) , (0x2, 0x4));
                case 3: // AB & BC (ABC)
                    return DualEdges((tri.a, tri.b, tri.c), (0x1, 0x2, 0x4));
                case 4: // CA
                    return Edge((tri.c, tri.a), (0x4, 0x1));
                case 5: // CA & AB (CAB)
                    return DualEdges((tri.c, tri.a, tri.b), (0x4, 0x1, 0x2));
                case 6: // CA & BC (BCA)
                    return DualEdges((tri.b, tri.c, tri.a), (0x2, 0x4, 0x1));
                default:
                    return 0;
            }        
        }

        public static int Barycentric2DVoronoi(
        (Vector3 a, Vector3 b, Vector3 c) tri,
        (int     a, int     b, int     c) bits,
            Vector3 p) {
            Vector3 abc  = Vector3.Cross(tri.c - tri.a, tri.b - tri.a);
                        
            // Gizmos.DrawRay((tri.a + tri.b + tri.c) / 3, abc);

            Vector3 ab_n = Vector3.Cross(abc, tri.b - tri.a);
            Vector3 bc_n = Vector3.Cross(abc, tri.c - tri.b);
            Vector3 ca_n = Vector3.Cross(abc, tri.a - tri.c);
            
            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            int ComputeSignedAreas(Vector3 p) {
                int nflags = 0;
                nflags |= Same(p - tri.a, ab_n) ? (1 << 0) : 0; // AB
                nflags |= Same(p - tri.b, bc_n) ? (1 << 1) : 0; // AB
                nflags |= Same(p - tri.c, ca_n) ? (1 << 2) : 0; // AB
                return nflags;
            }

            int DualEdges((Vector3 x, Vector3 y, Vector3 z) pm,
                          (int     x, int     y,     int z) bits) {
                // dual edges will serve as a permuted form of the triangle
                // xy, zy edges will be constructed for sign analysis
                Vector3 xy = pm.y - pm.x;
                Vector3 zy = pm.y - pm.z;

                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;
                Vector3 zp = p    - pm.z;

                Vector3 zxy = Vector3.Cross(zy, xy);

                Vector3 xy_n = Vector3.Cross(xy, zxy);
                Vector3 zy_n = Vector3.Cross(zxy, zy);

                
                if(Same(yp, zy) && Same(yp, xy))
                    return bits.y;

                if(!Same(yp, xy)) {
                    if(!Same(xp, xy))
                        return bits.x;
                    else
                        return bits.x | bits.y;
                }

                if(!Same(yp, zy)) {
                    if(!Same(zp, zy))
                        return bits.z;
                    else
                        return bits.y | bits.z;
                }

                return bits.x | bits.y | bits.z;
            }

            int Edge((Vector3 x, Vector3 y) pm, 
                     (int     x, int     y) bits) {
                Vector3 xy = pm.y - pm.x;
                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;

                if(Same(xy, yp) && Same(xy, xp))
                    return bits.y;
                
                if(!Same(xy, yp) && !Same(xy, xp))
                    return bits.x;

                return bits.x | bits.y;
            }

            switch(ComputeSignedAreas(p)) { 
                case 1: // AB
                    return Edge((tri.a, tri.b), (bits.a, bits.b));
                case 2: // BC
                    return Edge((tri.b, tri.c) , (bits.b, bits.c));
                case 3: // AB & BC (ABC)
                    return DualEdges((tri.a, tri.b, tri.c), (bits.a, bits.b, bits.c));
                case 4: // CA
                    return Edge((tri.c, tri.a), (bits.c, bits.a));
                case 5: // CA & AB (CAB)
                    return DualEdges((tri.c, tri.a, tri.b), (bits.c, bits.a, bits.b));
                case 6: // CA & BC (BCA)
                    return DualEdges((tri.b, tri.c, tri.a), (bits.b, bits.c, bits.a));
                default:
                    return bits.a | bits.b | bits.c;
            }        
        }
        
        public static (int r, Vector3 v) Barycentric2D_GJK(
        (Vector3 a, Vector3 b, Vector3 c) tri,
        (int     a, int     b, int     c) bits,
            Vector3 p) {
            Vector3 abc  = Vector3.Cross(tri.c - tri.a, tri.b - tri.a);
            float area = abc.magnitude;
            abc /= area;

            Vector3 ab_n = Vector3.Cross(abc, tri.b - tri.a);
            Vector3 bc_n = Vector3.Cross(abc, tri.c - tri.b);
            Vector3 ca_n = Vector3.Cross(abc, tri.a - tri.c);
            
            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };

            // Vector3 Bary(Vector3 p) {
            //     Vector3 v = new Vector3(
            //     Vector3.Dot(Vector3.Cross(p - tri.b, tri.c - p), abc) / area, // 0 -> ab x ao oab
            //     Vector3.Dot(Vector3.Cross(p - tri.c, tri.a - p), abc) / area, // 1 -> bc x bo obc
            //     0F);
            //     v[2] = 1 - v[0] - v[1];  // 2 -> ca x co oca
            //     return v;
            // }
            
            int ComputeSignedAreas(Vector3 p) {
                int nflags = 0;
                nflags |= Same(p - tri.a, ab_n) ? (1 << 0) : 0; // AB
                nflags |= Same(p - tri.b, bc_n) ? (1 << 1) : 0; // AB
                nflags |= Same(p - tri.c, ca_n) ? (1 << 2) : 0; // AB
                return nflags;
            }

            (int, Vector3) DualEdges((Vector3 x, Vector3 y, Vector3 z) pm,
                          (int     x, int     y,     int z) bits) {
                // dual edges will serve as a permuted form of the triangle
                // xy, zy edges will be constructed for sign analysis
                Vector3 xy = pm.y - pm.x;
                Vector3 zy = pm.y - pm.z;

                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;
                Vector3 zp = p    - pm.z;

                Vector3 zxy = Vector3.Cross(zy, xy);

                Vector3 xy_n = Vector3.Cross(xy, zxy);
                Vector3 zy_n = Vector3.Cross(zxy, zy);
                
                if(Same(yp, zy) && Same(yp, xy))
                    return (bits.y, (pm.y));

                if(!Same(yp, xy)) {
                    if(!Same(xp, xy))
                        return (bits.x, (pm.x));
                    else 
                        return (bits.x | bits.y, VectorHeader.ClosestPointEdge((pm.x, pm.y), p).b);
                }

                if(!Same(yp, zy)) {
                    if(!Same(zp, zy))
                        return (bits.z, (pm.z));
                    else {
                        return (bits.y | bits.z, VectorHeader.ClosestPointEdge((pm.y, pm.z), p).b);
                    }
                }
                
                return (bits.x | bits.y | bits.z, p - VectorHeader.ProjectVector(p - pm.x, zxy.normalized));
            }

            (int, Vector3) Edge((Vector3 x, Vector3 y) pm, 
                     (int     x, int     y) bits) {
                Vector3 xy = pm.y - pm.x;
                Vector3 yp = p    - pm.y;
                Vector3 xp = p    - pm.x;

                if(Same(xy, yp) && Same(xy, xp))
                    return (bits.y, (pm.y));
                
                if(!Same(xy, yp) && !Same(xy, xp))
                    return (bits.x, (pm.x));

                return (bits.x | bits.y, (pm.x + VectorHeader.ProjectVector(xp, xy.normalized)));
            }

            switch(ComputeSignedAreas(p)) { 
                case 1: // AB
                    return Edge((tri.a, tri.b), (bits.a, bits.b));
                case 2: // BC
                    return Edge((tri.b, tri.c) , (bits.b, bits.c));
                case 3: // AB & BC (ABC)
                    return DualEdges((tri.a, tri.b, tri.c), (bits.a, bits.b, bits.c));
                case 4: // CA
                    return Edge((tri.c, tri.a), (bits.c, bits.a));
                case 5: // CA & AB (CAB)
                    return DualEdges((tri.c, tri.a, tri.b), (bits.c, bits.a, bits.b));
                case 6: // CA & BC (BCA)
                    return DualEdges((tri.b, tri.c, tri.a), (bits.b, bits.c, bits.a));
                default:
                    return (bits.a | bits.b | bits.c, (p - VectorHeader.ProjectVector(p - tri.a, abc)));
            }        
        }
        
        public static Vector4 Barycentric3DClamped((Vector3 a, Vector3 b, Vector3 c, Vector3 d) tet, Vector3 o) {
            bool Same(Vector3 v1, Vector3 v2) {
                    return VectorHeader.Dot(v1, v2) > 0;
            };

            Vector3 a = tet.a;
            Vector3 b = tet.b;
            Vector3 c = tet.c;
            Vector3 d = tet.d;
            
            Vector3 adb = Vector3.Cross(d - a, b - d);
            Vector3 acd = Vector3.Cross(c - a, d - c);
            Vector3 abc = Vector3.Cross(b - a, c - b);
            Vector3 cbd = Vector3.Cross(b - c, d - b);

            float vol = Vector3.Dot(a - c, cbd);
            Vector4 Bary(Vector3 p) {
                Vector4 v = new Vector4(
                    Vector3.Dot(p - c, cbd) / vol, // a
                    Vector3.Dot(p - a, acd) / vol, // b
                    Vector3.Dot(p - d, adb) / vol, // c
                    0F
                );
                v[3] = 1 - v[2] - v[1] - v[0];
                return v;
                // test
            }

            Vector3 TripleEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, Vector3 p) {
                int nflags = 0;
                nflags |= Same(o - tet.x, tet.x - tet.y) ? (1 << 0) : 0;
                nflags |= Same(o - tet.x, tet.x - tet.z) ? (1 << 1) : 0;
                nflags |= Same(o - tet.x, tet.x - tet.w) ? (1 << 2) : 0;
            
                switch(nflags) {
                    default:
                        return Vector3.zero;
                    case 1: // red
                        return VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.z), o).b;
                        // XWZ
                    case 2: // blue
                        return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b;
                        // YWX
                    case 3: // red and blue
                        Vector3 xyw = Vector3.Cross(tet.x - tet.w, Vector3.Cross(tet.x - tet.w, tet.x - tet.y));
                        if(Same(o - tet.x, xyw))
                            return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b; 
                            // YWX
                        else
                            return VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.z), o).b;
                            // XWZ
                    case 4: // green
                        return VectorHeader.ClosestPointTriangle((tet.x, tet.z, tet.y), o).b;
                        // XZY
                    case 5: // red and green
                        Vector3 xzw = Vector3.Cross(tet.z - tet.x, Vector3.Cross(tet.x - tet.z, tet.x - tet.w));
                        if(Same(o - tet.x, xzw))
                            return VectorHeader.ClosestPointTriangle((tet.y, tet.x, tet.z), o).b;
                            // YXZ
                        else
                            // XWZ
                            return VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.z), o).b;
                    case 6: // blue and green
                        Vector3 ywx = Vector3.Cross(tet.y - tet.x, Vector3.Cross(tet.x - tet.z, tet.y - tet.x));
                        if(Same(o - tet.x, ywx))
                            // YWX
                            return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b;
                        else
                            return VectorHeader.ClosestPointTriangle((tet.y, tet.x, tet.z), o).b;
                            // YXZ
                    case 7: // red blue green
                        return tet.x;
                        // X
                }
            }

            Vector3 SingleEdges((Vector3 x, Vector3 y, Vector3 z) tet, Vector3 p) {
                return VectorHeader.ClosestPointTriangle((tet.x, tet.y, tet.z), o).b;
            }

            Vector3 DualEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, Vector3 p) {
                Vector3 zxy = Vector3.Cross(tet.x - tet.z, tet.y - tet.x);
                Vector3 xwy = Vector3.Cross(tet.w - tet.x, tet.y - tet.w);
                Vector3 xwy_n = Vector3.Cross(Vector3.Cross(zxy, xwy), xwy);
                Vector3 zxy_n = Vector3.Cross(zxy, Vector3.Cross(zxy, xwy));

                Vector3 xo = o - tet.x; 
                // see which side of crease we exist in
                if(!Same(xo, xwy_n) && !Same(xo, zxy_n))
                    return VectorHeader.ClosestPointEdge((tet.x, tet.y), o).b;
                else if(Same(xo, zxy_n))
                    return VectorHeader.ClosestPointTriangle((tet.z, tet.x, tet.y), o).b;
                else if(Same(xo, xwy_n))
                    return VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.y), o).b;

                return Vector3.zero; // should never happen
            }

            int ComputeSignBits(Vector3 p) {
                int nflags = 0;
                nflags |= VectorHeader.Dot(o - b, adb) > 0 ? (1 << 0) : 0;
                nflags |= VectorHeader.Dot(o - d, acd) > 0 ? (1 << 1) : 0;
                nflags |= VectorHeader.Dot(o - c, abc) > 0 ? (1 << 2) : 0;
                nflags |= VectorHeader.Dot(o - d, cbd) > 0 ? (1 << 3) : 0;
                return nflags;
            }

            switch (ComputeSignBits(o)) {
                case 1:  // ADB             (SINGULAR)
                    return Bary(SingleEdges((a, d, b), o));
                case 2:  // ACD             (SINGULAR)
                    return Bary(SingleEdges((a, c, d), o));
                case 3:  // ADB & ACD       (DUAL)
                    return Bary(DualEdges((a, d, b, c), o));
                case 4:  // ABC             (SINGULAR)
                    return Bary(SingleEdges((a, b, c), o));
                case 5:  // ABC & ADB (     DUAL)
                    return Bary(DualEdges((a, b, c, d), o));
                case 6:  // ACD & ABC       (DUAL)
                    return Bary(DualEdges((a, c, d, b), o));
                case 7:  // ADB & ACD & ABC (TRIPLE)
                    return Bary(TripleEdges((a, b, c, d), o));
                case 8:  // CBD             (SINGULAR)
                    return Bary(SingleEdges((c, b, d), o));
                case 9:  // ADB & CBD       (DUAL)
                    return Bary(DualEdges((b, d, c, a), o));
                case 10: // ACD & CBD       (DUAL)
                    return Bary(DualEdges((c, d, a, b), o));
                case 11: // ADB & ACD & CBD (TRIPLE)
                    return Bary(TripleEdges((d, b, c, a), o));
                case 12: // ABC & CBD       (DUAL)
                    return Bary(DualEdges((b, c, a, d), o));
                case 13: // ADB & ABC & CBD (TRIPLE)
                    return Bary(TripleEdges((b, a, c, d), o));
                case 14: // ACD & ABC & CBD (TRIPLE)
                    return Bary(TripleEdges((c, b, a, d), o));
                default:
                    return Bary(o);
            }
        }

        public static int Barycentric3DVoronoi(
            (Vector3 a, Vector3 b, Vector3 c, Vector3 d) tet, Vector3 o) {
            bool Same(Vector3 v1, Vector3 v2) {
                    return VectorHeader.Dot(v1, v2) > 0;
            };

            Vector3 a = tet.a;
            Vector3 b = tet.b;
            Vector3 c = tet.c;
            Vector3 d = tet.d;
            
            Vector3 adb = Vector3.Cross(d - a, b - d);
            Vector3 acd = Vector3.Cross(c - a, d - c);
            Vector3 abc = Vector3.Cross(b - a, c - b);
            Vector3 cbd = Vector3.Cross(b - c, d - b);

            int TripleEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, (int x, int y, int z, int w) bits, Vector3 p) {
                int nflags = 0;
                nflags |= Same(o - tet.x, tet.x - tet.y) ? (1 << 0) : 0;
                nflags |= Same(o - tet.x, tet.x - tet.z) ? (1 << 1) : 0;
                nflags |= Same(o - tet.x, tet.x - tet.w) ? (1 << 2) : 0;
            
                switch(nflags) {
                    default:
                        return 0;
                    case 1: // red
                        // return VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.z), o).b;
                        return VectorHeader.Barycentric2DVoronoi((tet.x, tet.w, tet.z), (bits.x, bits.w, bits.z), o);
                        // XWZ
                    case 2: // blue
                        // return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b;
                        return VectorHeader.Barycentric2DVoronoi((tet.y, tet.w, tet.x), (bits.y, bits.w, bits.x), o);
                        // YWX
                    case 3: // red and blue
                        Vector3 xyw = Vector3.Cross(tet.x - tet.w, Vector3.Cross(tet.x - tet.w, tet.x - tet.y));
                        if(Same(o - tet.x, xyw))
                            // return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b; 
                            return VectorHeader.Barycentric2DVoronoi((tet.y, tet.w, tet.x), (bits.y, bits.w, bits.x), o); 
                            // YWX
                        else
                            return VectorHeader.Barycentric2DVoronoi((tet.x, tet.w, tet.z), (bits.x, bits.w, bits.z), o);
                            // XWZ
                    case 4: // green
                        return VectorHeader.Barycentric2DVoronoi((tet.x, tet.z, tet.y), (bits.x, bits.z, bits.y), o);
                        // XZY
                    case 5: // red and green
                        Vector3 xzw = Vector3.Cross(tet.z - tet.x, Vector3.Cross(tet.x - tet.z, tet.x - tet.w));
                        if(Same(o - tet.x, xzw))
                            return VectorHeader.Barycentric2DVoronoi((tet.y, tet.x, tet.z), (bits.y, bits.x, bits.z), o);
                            // YXZ
                        else
                            // XWZ
                            return VectorHeader.Barycentric2DVoronoi((tet.x, tet.w, tet.z), (bits.x, bits.w, bits.z), o);
                    case 6: // blue and green
                        Vector3 ywx = Vector3.Cross(tet.y - tet.x, Vector3.Cross(tet.x - tet.z, tet.y - tet.x));
                        if(Same(o - tet.x, ywx))
                            // YWX
                            return VectorHeader.Barycentric2DVoronoi((tet.y, tet.w, tet.x), (bits.y, bits.w, bits.x), o);
                        else
                            return VectorHeader.Barycentric2DVoronoi((tet.y, tet.x, tet.z), (bits.y, bits.x, bits.z), o);
                            // YXZ
                    case 7: // red blue green
                        return bits.x;
                        // X
                }
            }

            int SingleEdges((Vector3 x, Vector3 y, Vector3 z) tet, (int x, int y, int z) bits, Vector3 p) {
                return VectorHeader.Barycentric2DVoronoi((tet.x, tet.y, tet.z), (bits.x, bits.y, bits.z), o);
            }

            int DualEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, (int x, int y, int z, int w) bits, Vector3 p) {
                Vector3 zxy = Vector3.Cross(tet.x - tet.z, tet.y - tet.x);
                Vector3 xwy = Vector3.Cross(tet.w - tet.x, tet.y - tet.w);
                Vector3 xwy_n = Vector3.Cross(Vector3.Cross(zxy, xwy), xwy);
                Vector3 zxy_n = Vector3.Cross(zxy, Vector3.Cross(zxy, xwy));

                Vector3 xo = o - tet.x; 
                // see which side of crease we exist in
                if(!Same(xo, xwy_n) && !Same(xo, zxy_n))
                    return VectorHeader.Barycentric1DVoronoi((tet.x, tet.y), (bits.x, bits.y), o);
                else if(Same(xo, zxy_n))
                    return VectorHeader.Barycentric2DVoronoi((tet.z, tet.x, tet.y), (bits.z, bits.x, bits.y), o);
                else if(Same(xo, xwy_n))
                    return VectorHeader.Barycentric2DVoronoi((tet.x, tet.w, tet.y), (bits.x, bits.w, bits.y), o);

                return 0; // should never happen
            }

            int ComputeSignBits(Vector3 p) {
                int nflags = 0;
                nflags |= VectorHeader.Dot(o - b, adb) > 0 ? (1 << 0) : 0;
                nflags |= VectorHeader.Dot(o - d, acd) > 0 ? (1 << 1) : 0;
                nflags |= VectorHeader.Dot(o - c, abc) > 0 ? (1 << 2) : 0;
                nflags |= VectorHeader.Dot(o - d, cbd) > 0 ? (1 << 3) : 0;
                return nflags;
            }

            switch (ComputeSignBits(o)) {
                case 1:  // ADB             (SINGULAR)
                    return SingleEdges((a, d, b),       (0x1, 0x8, 0x2), o);
                case 2:  // ACD             (SINGULAR)
                    return SingleEdges((a, c, d),       (0x1, 0x4, 0x8), o);
                case 3:  // ADB & ACD       (DUAL)
                    return DualEdges((a, d, b, c),      (0x1, 0x8, 0x2, 0x4), o);
                case 4:  // ABC             (SINGULAR)
                    return SingleEdges((a, b, c),       (0x1, 0x2, 0x4), o);
                case 5:  // ABC & ADB (     DUAL)
                    return DualEdges((a, b, c, d),      (0x1, 0x2, 0x4, 0x8), o);
                case 6:  // ACD & ABC       (DUAL)
                    return DualEdges((a, c, d, b),      (0x1, 0x4, 0x8, 0x2), o);
                case 7:  // ADB & ACD & ABC (TRIPLE)
                    return TripleEdges((a, b, c, d),    (0x1, 0x2, 0x4, 0x8), o);
                case 8:  // CBD             (SINGULAR)
                    return SingleEdges((c, b, d),       (0x4, 0x2, 0x8), o);
                case 9:  // ADB & CBD       (DUAL)
                    return DualEdges((b, d, c, a),      (0x2, 0x8, 0x4, 0x1), o);
                case 10: // ACD & CBD       (DUAL)
                    return DualEdges((c, d, a, b),      (0x4, 0x8, 0x1, 0x2), o);
                case 11: // ADB & ACD & CBD (TRIPLE)
                    return TripleEdges((d, b, c, a),    (0x8, 0x2, 0x4, 0x1), o);
                case 12: // ABC & CBD       (DUAL)
                    return DualEdges((b, c, a, d),      (0x2, 0x4, 0x1, 0x8), o);
                case 13: // ADB & ABC & CBD (TRIPLE)
                    return TripleEdges((b, a, c, d),    (0x2, 0x1, 0x4, 0x8), o);
                case 14: // ACD & ABC & CBD (TRIPLE)
                    return TripleEdges((c, b, a, d),    (0x4, 0x2, 0x1, 0x8), o);
                default:
                    return 0;
            }
        }

        public static (int, Vector4) Barycentric3D_GJK(
            (Vector3 a, Vector3 b, Vector3 c, Vector3 d) tet, Vector3 o) {
            bool Same(Vector3 v1, Vector3 v2) {
                    return VectorHeader.Dot(v1, v2) > 0;
            };

            Vector3 a = tet.a;
            Vector3 b = tet.b;
            Vector3 c = tet.c;
            Vector3 d = tet.d;
            
            Vector3 adb = Vector3.Cross(d - a, b - d);
            Vector3 acd = Vector3.Cross(c - a, d - c);
            Vector3 abc = Vector3.Cross(b - a, c - b);
            Vector3 cbd = Vector3.Cross(b - c, d - b);

            // THEY ARE ALL FLIPPED
            // if(DistanceGJK.iteration == DistanceGJK.stopat) {
            //     Gizmos.color = Color.magenta;
            //     Gizmos.DrawRay(a, adb / 10);
            //     Gizmos.DrawRay(d, adb / 10);
            //     Gizmos.DrawRay(b, adb / 10);
            //     Gizmos.color = Color.yellow;
            //     Gizmos.DrawRay(a, acd / 5);
            //     Gizmos.DrawRay(c, acd / 5);
            //     Gizmos.DrawRay(d, acd / 5);
            //     Gizmos.color = Color.cyan;
            //     Gizmos.DrawRay(a, abc / 5);
            //     Gizmos.DrawRay(b, abc / 5);
            //     Gizmos.DrawRay(c, abc / 5);
            //     Gizmos.color = Color.green;
            //     Gizmos.DrawRay(c, cbd / 5);
            //     Gizmos.DrawRay(b, cbd / 5);
            //     Gizmos.DrawRay(d, cbd / 5);
            //     Gizmos.color = Color.white;
            // }

            float vol = Vector3.Dot(a - c, cbd);
            Vector4 Bary(Vector3 p) {
                Vector4 v = new Vector4(
                    Vector3.Dot(p - c, cbd) / vol, // a
                    Vector3.Dot(p - a, acd) / vol, // b
                    Vector3.Dot(p - d, adb) / vol, // c
                    0F
                );
                v[3] = 1 - v[2] - v[1] - v[0];
                return v;
            }

            (int, Vector3) TripleEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, (int x, int y, int z, int w) bits, Vector3 p) {
                int nflags = 0;
                nflags |= Same(o - tet.x, tet.x - tet.y) ? (1 << 0) : 0;
                nflags |= Same(o - tet.x, tet.x - tet.z) ? (1 << 1) : 0;
                nflags |= Same(o - tet.x, tet.x - tet.w) ? (1 << 2) : 0;
            
                // Gizmos.color = Color.red;
                // Gizmos.DrawRay(tet.x, tet.x - tet.y); // red
                // Gizmos.color = Color.green;
                // Gizmos.DrawRay(tet.x, tet.x - tet.z); // gr
                // Gizmos.color = Color.blue;
                // Gizmos.DrawRay(tet.x, tet.x - tet.w); // bl
                // Gizmos.color = Color.white;

                // Debug.Log(nflags);
                switch(nflags) {
                    default:
                        // Debug.Log("OK");
                        return (0, Vector3.zero);
                    case 1: // red
                        // return VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.z), o).b;
                        return VectorHeader.Barycentric2D_GJK((tet.x, tet.w, tet.z), (bits.x, bits.w, bits.z), o);
                        // XWZ
                    case 2: // blue
                        // return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b;
                        return VectorHeader.Barycentric2D_GJK((tet.y, tet.w, tet.x), (bits.y, bits.w, bits.x), o);
                        // YWX
                    case 3: // red and blue
                        Vector3 xyw = Vector3.Cross(tet.x - tet.w, Vector3.Cross(tet.x - tet.w, tet.x - tet.y));
                        if(Same(o - tet.x, xyw))
                            // return VectorHeader.ClosestPointTriangle((tet.y, tet.w, tet.x), o).b; 
                            return VectorHeader.Barycentric2D_GJK((tet.y, tet.w, tet.x), (bits.y, bits.w, bits.x), o); 
                            // YWX
                        else
                            return VectorHeader.Barycentric2D_GJK((tet.x, tet.w, tet.z), (bits.x, bits.w, bits.z), o);
                            // XWZ
                    case 4: // green
                        return VectorHeader.Barycentric2D_GJK((tet.x, tet.z, tet.y), (bits.x, bits.z, bits.y), o);
                        // XZY
                    case 5: // red and green
                        Vector3 xzw = Vector3.Cross(tet.z - tet.x, Vector3.Cross(tet.x - tet.z, tet.x - tet.w));
                        if(Same(o - tet.x, xzw))
                            return VectorHeader.Barycentric2D_GJK((tet.y, tet.x, tet.z), (bits.y, bits.x, bits.z), o);
                            // YXZ
                        else
                            // XWZ
                            return VectorHeader.Barycentric2D_GJK((tet.x, tet.w, tet.z), (bits.x, bits.w, bits.z), o);
                    case 6: // blue and green
                        Vector3 ywx = Vector3.Cross(tet.y - tet.x, Vector3.Cross(tet.x - tet.z, tet.y - tet.x));
                        if(Same(o - tet.x, ywx)) {
                            // YWX
                            return VectorHeader.Barycentric2D_GJK((tet.y, tet.w, tet.x), (bits.y, bits.w, bits.x), o);
                        }
                        else {
                            return VectorHeader.Barycentric2D_GJK((tet.y, tet.x, tet.z), (bits.y, bits.x, bits.z), o);
                        }
                            // YXZ
                    case 7: // red blue green
                        return (bits.x, tet.x);
                        // X
                }
            }

            (int, Vector3) SingleEdges((Vector3 x, Vector3 y, Vector3 z) tet, (int x, int y, int z) bits, Vector3 p) {
                return VectorHeader.Barycentric2D_GJK((tet.x, tet.y, tet.z), (bits.x, bits.y, bits.z), o);
            }

            (int, Vector3) DualEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, (int x, int y, int z, int w) bits, Vector3 p) {
                Vector3 zxy = Vector3.Cross(tet.x - tet.z, tet.y - tet.x);
                Vector3 xwy = Vector3.Cross(tet.w - tet.x, tet.y - tet.w);
                Vector3 xwy_n = Vector3.Cross(Vector3.Cross(zxy, xwy), xwy);
                Vector3 zxy_n = Vector3.Cross(zxy, Vector3.Cross(zxy, xwy));

                // Gizmos.color = Color.cyan;
                // Gizmos.DrawRay(tet.x, xwy_n);
                // Gizmos.DrawRay(tet.x, zxy_n);
                // Gizmos.color = Color.white;

                Vector3 xo = o - tet.x;
                // see which side of crease we exist in
                if(!Same(xo, xwy_n) && !Same(xo, zxy_n))
                    return VectorHeader.Barycentric1D_GJK((tet.x, tet.y), (bits.x, bits.y), o);
                else if(Same(xo, zxy_n))
                    return VectorHeader.Barycentric2D_GJK((tet.z, tet.x, tet.y), (bits.z, bits.x, bits.y), o);
                else if(Same(xo, xwy_n))
                    return VectorHeader.Barycentric2D_GJK((tet.x, tet.w, tet.y), (bits.x, bits.w, bits.y), o);

                return (0, Vector3.zero); // should never happen
            }

            int ComputeSignBits(Vector3 p) {
                int nflags = 0;
                nflags |= VectorHeader.Dot(o - b, adb) > 0 ? (1 << 0) : 0;
                nflags |= VectorHeader.Dot(o - d, acd) > 0 ? (1 << 1) : 0;
                nflags |= VectorHeader.Dot(o - c, abc) > 0 ? (1 << 2) : 0;
                nflags |= VectorHeader.Dot(o - d, cbd) > 0 ? (1 << 3) : 0;
                return nflags;
            }

            (int reg, Vector3 v) vec = (0, Vector3.zero);

            int v = ComputeSignBits(o);
            // Debug.Log(v);
            switch (v) {
                case 1:  // ADB             (SINGULAR)
                    vec = SingleEdges((a, d, b),       (0x1, 0x8, 0x2), o);
                    break;
                case 2:  // ACD             (SINGULAR)
                    vec = SingleEdges((a, c, d),       (0x1, 0x4, 0x8), o);
                    break;
                case 3:  // ADB & ACD       (DUAL)
                    vec = DualEdges((a, d, b, c),      (0x1, 0x8, 0x2, 0x4), o);
                    break;
                case 4:  // ABC             (SINGULAR)
                    vec = SingleEdges((a, b, c),       (0x1, 0x2, 0x4), o);
                    break;
                case 5:  // ABC & ADB (     DUAL)
                    vec = DualEdges((a, b, c, d),      (0x1, 0x2, 0x4, 0x8), o);
                    break;
                case 6:  // ACD & ABC       (DUAL)
                     vec = DualEdges((a, c, d, b),      (0x1, 0x4, 0x8, 0x2), o);
                    break;
                case 7:  // ADB & ACD & ABC (TRIPLE)
                    vec = TripleEdges((a, b, c, d),    (0x1, 0x2, 0x4, 0x8), o);
                    break;
                case 8:  // CBD             (SINGULAR)
                    vec = SingleEdges((c, b, d),       (0x4, 0x2, 0x8), o);
                    break;
                case 9:  // ADB & CBD       (DUAL)
                    vec = DualEdges((b, d, c, a),      (0x2, 0x8, 0x4, 0x1), o);
                    break;
                case 10: // ACD & CBD       (DUAL)
                    vec = DualEdges((c, d, a, b),      (0x4, 0x8, 0x1, 0x2), o);
                    break;
                case 11: // ADB & ACD & CBD (TRIPLE)
                    vec = TripleEdges((d, b, c, a),    (0x8, 0x2, 0x4, 0x1), o);
                    break;
                case 12: // ABC & CBD       (DUAL)
                    vec = DualEdges((b, c, a, d),      (0x2, 0x4, 0x1, 0x8), o);
                    break;
                case 13: // ADB & ABC & CBD (TRIPLE)
                    vec = TripleEdges((b, a, c, d),    (0x2, 0x1, 0x4, 0x8), o);
                    break;
                case 14: // ACD & ABC & CBD (TRIPLE)
                    vec = TripleEdges((c, b, a, d),    (0x4, 0x2, 0x1, 0x8), o);
                    break;
                default:
                    vec = (0, o);
                break;
            }

            return (vec.reg, vec.v);
        }
    }
}