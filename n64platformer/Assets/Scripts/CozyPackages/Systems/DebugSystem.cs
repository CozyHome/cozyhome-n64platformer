namespace com.cozyhome.Systems
{
    public class DebugSystem : UnityEngine.MonoBehaviour,
        SystemsHeader.IDiscoverSystem,
        SystemsHeader.IFixedSystem,
        SystemsHeader.ILateUpdateSystem,
        SystemsHeader.IUpdateSystem
    {
        [UnityEngine.SerializeField] short _executionindex = 0;
        public void OnDiscover()
        {
            SystemsInjector.RegisterUpdateSystem(_executionindex,this);
            SystemsInjector.RegisterFixedSystem(_executionindex, this);
            SystemsInjector.RegisterLateSystem(_executionindex, this);
        }

        public void OnFixedUpdate() { }
        public void OnLateUpdate() { }
        public void OnUpdate() { }
    }
}