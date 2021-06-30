using UnityEngine;
using System;

namespace com.cozyhome.Timers
{
    public static class TimerHeader
    {
        [System.Serializable]
        public class DeltaTimer
        {
            [SerializeField] private float TotalElapsedTime;
            private float elapsed;
            public void Reset() => elapsed = 0.0F;
            public void Accumulate(float dt) => elapsed += dt;
            public void Max(float newmax) => TotalElapsedTime = newmax;
            public bool Check() => elapsed > TotalElapsedTime;
            public float Elapsed => elapsed;
            public float NormalizedElapsed => (elapsed / TotalElapsedTime);
        }
        
        [System.Serializable]
        public class SnapshotTimer
        {
            [SerializeField] private float TotalDeltaDifference;
            private float timestamp;

            public void Stamp(float timestamp) => this.timestamp = timestamp;
            public bool Check(float time) => (time - timestamp) > TotalDeltaDifference;
            public float Difference(float time) => (time - timestamp);
        }
    }
}