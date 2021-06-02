using com.cozyhome.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace com.cozyhome.Systems
{
    public static class SystemsHeader
    {
        public interface IDiscoverSystem { void OnDiscover(); }
        public interface IUpdateSystem { void OnUpdate(); }
        public interface IFixedSystem { void OnFixedUpdate(); }
        public interface ILateUpdateSystem { void OnLateUpdate(); }
    }

    [DefaultExecutionOrder(-500)]
    public partial class SystemsInjector : SingletonBehaviour<SystemsInjector>
    {
        [SerializeField] private bool SimulateFixed;
        [SerializeField] private bool SimulateUpdate;
        [SerializeField] private bool SimulateLate;

        private SortedList<short, SystemsHeader.IFixedSystem> _fixedsystems = null;
        private SortedList<short, SystemsHeader.IUpdateSystem> _updatesystems = null;
        private SortedList<short, SystemsHeader.ILateUpdateSystem> _latesystems = null;

        protected override void OnAwake()
        {
            _fixedsystems = new SortedList<short, SystemsHeader.IFixedSystem>();
            _updatesystems = new SortedList<short, SystemsHeader.IUpdateSystem>();
            _latesystems = new SortedList<short, SystemsHeader.ILateUpdateSystem>();

            // Get System Components
            SystemsHeader.IDiscoverSystem[] _discoveredsystems =
                this.GetComponents<SystemsHeader.IDiscoverSystem>();

            for (int i = _discoveredsystems.Length - 1; i >= 0; i--)
                _discoveredsystems[i].OnDiscover();

            _discoveredsystems = null;
        }

        public void Update()
        {
            if (!SimulateUpdate)
                return;
            else
            {
                for (int i = 0; i < _updatesystems.Count; i++)
                    _updatesystems.Values[i].OnUpdate();
                return;
            }
        }

        public void FixedUpdate()
        {
            if (!SimulateFixed)
                return;
            else
            {
                for (int i = 0; i < _fixedsystems.Count; i++)
                    _fixedsystems.Values[i].OnFixedUpdate();
                return;
            }
        }

        public void LateUpdate()
        {
            if (!SimulateLate)
                return;
            else
            {
                for (int i = 0; i < _latesystems.Count; i++)
                    _latesystems.Values[i].OnLateUpdate();
                return;
            }
        }

        public static void RegisterUpdateSystem(short _executionindex, SystemsHeader.IUpdateSystem _sys) => Instance._updatesystems?.Add(_executionindex, _sys);
        public static void RemoveUpdateSystem(short _executionindex) => Instance._updatesystems?.RemoveAt(_executionindex);

        public static void RegisterFixedSystem(short _executionindex, SystemsHeader.IFixedSystem _sys) => Instance._fixedsystems?.Add(_executionindex, _sys);
        public static void RemoveFixedSystem(short _executionindex) => Instance._fixedsystems?.RemoveAt(_executionindex);

        public static void RegisterLateSystem(short _executionindex, SystemsHeader.ILateUpdateSystem _sys) => Instance._latesystems?.Add(_executionindex, _sys);
        public static void RemoveLateSystem(short _executionindex, SystemsHeader.ILateUpdateSystem _sys) => Instance._latesystems?.RemoveAt(_executionindex);
    }
}
