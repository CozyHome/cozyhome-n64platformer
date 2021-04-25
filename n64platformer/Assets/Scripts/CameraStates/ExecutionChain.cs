using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionChain<T1, T2>
{
    private List<Execution> Executions;
    private Dictionary<T1, Execution> ExecutionRegistry;
    private T2 Middleman;

    public ExecutionChain(T2 Middleman)
    {
        this.Middleman = Middleman;

        Executions = new List<Execution>();
        ExecutionRegistry = new Dictionary<T1, Execution>();
    }

    public void AddExecution(Execution newexecution)
    {
        if (ExecutionRegistry.ContainsKey(newexecution.GetKey))
            return;
        else
        {
            newexecution.Enter(Middleman);

            Executions.Add(newexecution);
            ExecutionRegistry.Add(newexecution.GetKey, newexecution);
        }
    }

    public void Tick()
    {
        for (int i = 0; i < Executions.Count; i++)
        {
            Execution current = Executions[i];
            if (current == null)
                continue;
            else
            {
                if (current.Execute(Middleman))
                    continue;
                else
                {
                    current.Exit(Middleman);

                    Executions.RemoveAt(i);
                    ExecutionRegistry.Remove(current.GetKey);
                }
            }
        }
    }

    public abstract class Execution
    {
        [SerializeField] protected T1 key;

        public abstract bool Execute(T2 Middleman);
        public T1 GetKey => key;

        public abstract void Enter(T2 Middleman);
        public abstract void Exit(T2 Middleman);
    }
}