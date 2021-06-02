using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.cozyhome.ConcurrentExecution
{
    public class DebugExecutionBehaviour : ConcurrentHeader.ExecutionMachine<Middleman>.MonoExecution
    {
        private JumpExecution _Jump = new JumpExecution();

        public override void OnBehaviourDiscovered(
            Action<ConcurrentHeader.ExecutionMachine<Middleman>.ConcurrentExecution>[] ExecutionCommands,
            Middleman middleman)
        {
            _Jump.OnBaseDiscovery(ExecutionCommands, middleman);
        }
    }

    public class JumpExecution : ConcurrentHeader.ExecutionMachine<Middleman>.ConcurrentExecution
    {
        protected override void OnExecutionDiscovery(Middleman middleman)
        {
            RegisterExecution();
            BeginExecution();
        }

        public override void Simulate(Middleman _args)
        {

        }
    }
}