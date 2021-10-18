namespace com.cozyhome.Systems
{
    public class DebugSystem : UnityEngine.MonoBehaviour,
        SystemsHeader.IDiscoverSystem,
        SystemsHeader.IFixedSystem,
        SystemsHeader.ILateUpdateSystem,
        SystemsHeader.IUpdateSystem
    {
        [UnityEngine.SerializeField] PlayerMachine p_machine;
        [UnityEngine.SerializeField] CameraMachine c_machine;

        [UnityEngine.SerializeField] short _executionindex = 0;
        public void OnDiscover()
        {
            SystemsInjector.RegisterUpdateSystem(_executionindex,this);
            SystemsInjector.RegisterFixedSystem(_executionindex, this);
            SystemsInjector.RegisterLateSystem(_executionindex, this);
        }

        public void OnFixedUpdate() 
        {
            p_machine.F_Update();
            c_machine.F_Update();
        }
        
        public void OnUpdate() 
        {
            c_machine.U_Update();
        }

        public void OnLateUpdate() { }
    }
}