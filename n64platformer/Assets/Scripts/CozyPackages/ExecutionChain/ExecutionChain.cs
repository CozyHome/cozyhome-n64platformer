using System.Collections.Generic;
using UnityEngine;

namespace com.cozyhome.ChainedExecutions
{
    public enum ExecutionState { ACTIVE = 1, FINISHED = 0 }
    public class ExecutionChain<T1, T2>
    {
        private Stack<T1> RemoveIndices;
        private SortedList<T1, Execution> Executions;
        private Dictionary<T1, Execution> ExecutionRegistry;
        private T2 Middleman;

        public ExecutionChain(T2 Middleman)
        {
            this.Middleman = Middleman;

            RemoveIndices = new Stack<T1>();
            Executions = new SortedList<T1, Execution>();
            ExecutionRegistry = new Dictionary<T1, Execution>();
        }

        public void AddExecution(Execution newexecution)
        {
            if (ExecutionRegistry.ContainsKey(newexecution.GetKey))
                return;
            else
            {
                newexecution.Enter(Middleman);
                Executions.Add(newexecution.GetKey, newexecution);
                ExecutionRegistry.Add(newexecution.GetKey, newexecution);
            }
        }

        public void EndExecution(T1 key)
        {
            if (!ExecutionRegistry.ContainsKey(key))
                return;
            else
            {
                Executions[key].Exit(Middleman);
                Executions.Remove(key);
                ExecutionRegistry.Remove(key);
            }
        }

        public bool IsExecutionActive(T1 key) => ExecutionRegistry.ContainsKey(key);

        public void Tick()
        {
            IList<Execution> ExecutionValues = Executions.Values;
            for (int i = 0; i < ExecutionValues.Count; i++)
            {
                Execution current = ExecutionValues[i];
                if (current == null) // if empty skip
                    continue;
                else
                {
                    if (current.Execute(Middleman) == ExecutionState.FINISHED)
                        RemoveIndices.Push(current.GetKey);
                    else
                        continue;
                }
            }

            // remove
            while (RemoveIndices.Count > 0)
            {
                T1 key = RemoveIndices.Pop();
                EndExecution(key);
            }
        }

        public abstract class Execution
        {
            public Execution(T1 key) { this.key = key; }
            public Execution() { } // for Unity

            [SerializeField] protected T1 key;
            public abstract ExecutionState Execute(T2 Middleman);
            public abstract void Enter(T2 Middleman);
            public abstract void Exit(T2 Middleman);
            public T1 GetKey => key;
        }
    }
}