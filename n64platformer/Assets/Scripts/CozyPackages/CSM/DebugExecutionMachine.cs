using UnityEngine;

namespace com.cozyhome.ConcurrentExecution
{
    [System.Serializable] public class Middleman
    {
        // Put data and funcs in here
    }

    public class DebugExecutionMachine : ConcurrentHeader.ExecutionMachine<Middleman>
    {
        [SerializeField] Middleman middleman;
        void Start()
        {
            this.Initialize(middleman);
        }
    }
}
