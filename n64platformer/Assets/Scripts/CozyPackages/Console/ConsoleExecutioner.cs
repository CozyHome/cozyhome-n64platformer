using com.cozyhome.ConcurrentExecution;
using UnityEngine;

namespace com.cozyhome.Console
{
    public class ConsoleExecutioner : ConcurrentHeader.ExecutionMachine<ConsoleArgs>
    {
        [SerializeField] ConsoleArgs _consoleargs;
        private void Awake()
        {
            base.Initialize(_consoleargs);
        }
        
        private void Update() 
        {
            base.Simulate(_consoleargs);
        }
    }
}