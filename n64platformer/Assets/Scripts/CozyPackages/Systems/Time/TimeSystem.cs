using UnityEngine;

namespace com.cozyhome.Systems
{
    public class TimeSystem : MonoBehaviour,
    SystemsHeader.IDiscoverSystem,
    SystemsHeader.IFixedSystem,
    SystemsHeader.ILateUpdateSystem,
    SystemsHeader.IUpdateSystem
    {
        [SerializeField] private short _executionindex = 1;

        public void OnDiscover()
        {
            for (int i = 0; i < 3; i++)
                GlobalTime.Apply(i, 0F, 0F);

            SystemsInjector.RegisterUpdateSystem(_executionindex, this);
            SystemsInjector.RegisterFixedSystem(_executionindex, this);
            SystemsInjector.RegisterLateSystem(_executionindex, this);
        }

        public void OnUpdate()
        =>
            GlobalTime.Apply(0, Time.deltaTime, Time.time);
        public void OnFixedUpdate()
        =>
            GlobalTime.Apply(1, Time.fixedDeltaTime, Time.fixedTime);
        public void OnLateUpdate()
        =>
            GlobalTime.Apply(2, Time.deltaTime, Time.time);

    }
}

