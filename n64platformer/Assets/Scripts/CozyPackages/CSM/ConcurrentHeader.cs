using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.cozyhome.ConcurrentExecution
{

    public static class ConcurrentHeader
    {
        public class ExecutionMachine<TMiddleman> : MonoBehaviour
        {
            private Action<ConcurrentExecution>[] _executioncommands;

            private Dictionary<string, ConcurrentExecution> _executionindex;
            private SortedList<int, ConcurrentExecution> _simulatingexecutions;

            public void Initialize(TMiddleman _middleman)
            {
                _executioncommands = new Action<ConcurrentExecution>[4]
                {
                InsertInIndex,
                RemoveFromIndex,
                InsertInSimulation,
                RemoveFromSimulation
                };

                _executionindex = new Dictionary<string, ConcurrentExecution>();
                _simulatingexecutions = new SortedList<int, ConcurrentExecution>();

                Func<ExecutionMachine<TMiddleman>.ConcurrentExecution, bool> _indexattachment;
                _indexattachment = (_execution) =>
                {
                    string _key = _execution.Key;
                    if (!_executionindex.ContainsKey(_key))
                    {
                        _executionindex.Add(_key, _execution);
                        return true;
                    }
                    else
                        return false;
                };

                Func<ExecutionMachine<TMiddleman>.ConcurrentExecution, bool> _currentattachment;
                _currentattachment = (_execution) =>
                {
                    int _index = _execution.Offset;
                    if (!_simulatingexecutions.ContainsKey(_index))
                    {
                        _simulatingexecutions.Add(_index, _execution);
                        return true;
                    }
                    else
                        return false;
                };

                MonoExecution[] _behavioursfound = gameObject.GetComponents<MonoExecution>();

                for (int i = 0; i < _behavioursfound.Length; i++)
                    _behavioursfound[i].OnBehaviourDiscovered(_executioncommands, _middleman);

                _currentattachment = null;
                _indexattachment = null;
            }

            public void Simulate(TMiddleman middleman)
            {
                // Execution State
                var _executions = _simulatingexecutions.Values;
                for (int i = 0; i < _executions.Count; i++)
                    _executions[i].Simulate(middleman);
            }

            private void InsertInIndex(ConcurrentExecution _execution)
            => _executionindex.Add(_execution.Key, _execution);
            private void RemoveFromIndex(ConcurrentExecution _execution)
                => _executionindex.Add(_execution.Key, _execution);

            private void RemoveFromSimulation(ConcurrentExecution _execution)
            {
                if (_simulatingexecutions.ContainsKey(_execution.Offset))
                    _simulatingexecutions.Remove(_execution.Offset);
            }

            private void InsertInSimulation(ConcurrentExecution _execution)
            {
                if (!_simulatingexecutions.ContainsKey(_execution.Offset))
                    _simulatingexecutions.Add(_execution.Offset, _execution);
            }

            public abstract class MonoExecution : MonoBehaviour
            {
                public abstract void OnBehaviourDiscovered(Action<ConcurrentExecution>[] ExecutionCommands, TMiddleman Middleman);
            }

            public abstract class ConcurrentExecution
            {
                protected abstract void OnExecutionDiscovery(TMiddleman Middleman);

                [Header("Execution State Parameters")]
                [SerializeField] private string _key = "NIL";
                [SerializeField] private int _executionOffset = -1;
                [System.NonSerialized] private Action<ConcurrentExecution>[] _executioncommands;
                public void OnBaseDiscovery(Action<ConcurrentExecution>[] ExecutionCommands,
                                            TMiddleman Middleman)
                {
                    _executioncommands = ExecutionCommands;
                    this.OnExecutionDiscovery(Middleman);
                }

                public abstract void Simulate(TMiddleman _args);
                public string Key => _key;
                public int Offset => _executionOffset;

                public void RegisterExecution() => _executioncommands[0](this);
                public void UnregisterExecution() => _executioncommands[1](this);
                public void BeginExecution() => _executioncommands[2](this);
                public void EndExecution() => _executioncommands[3](this);
            }
        }
    }
}