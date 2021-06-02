using com.cozyhome.Singleton;
using UnityEngine;


namespace com.cozyhome.Systems
{
    [DefaultExecutionOrder(-1000)] class GlobalTime : SingletonBehaviour<GlobalTime>
    {
        private float[] _times = new float[3] { -1F, -1F, -1F };
        private float[] _deltas = new float[3] { -1F, -1F, -1F };

        public static void ApplyTime(int _i0, float _time) => Instance._times[_i0] = _time;
        public static void ApplyDelta(int _i0, float _delta) => Instance._deltas[_i0] = _delta;

        public static void Apply(int _i0, float _delta, float _time) 
        {
            Instance._times[_i0] = _time;
            Instance._deltas[_i0] = _delta;
        }

        protected override void OnAwake()
        {
            // Whatever we want ..?  :)
        }

        public static float T => Instance._times[0];
        public static float FT => Instance._times[1];
        public static float LT => Instance._times[2];

        public static float DT => Instance._deltas[0];
        public static float FDT => Instance._deltas[1];
        public static float LDT => Instance._deltas[2];

    }
}

