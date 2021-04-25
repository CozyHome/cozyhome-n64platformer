using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExecutionHeader
{

    [System.Serializable]
    public class OnJumpExecution : ExecutionChain<int, MainMiddleman>.Execution
    {
        [SerializeField] private AnimationCurve JumpCurve;
        [SerializeField] private float MaxJumpTime;
        [SerializeField] private float BounceHeight = 2F;
        private float JumpTime;

        public override void Enter(MainMiddleman Middleman) { JumpTime = 0F; }

        public override void Exit(MainMiddleman Middleman) { JumpTime = 0F; }

        public override bool Execute(MainMiddleman Middleman)
        {
            if (JumpTime >= MaxJumpTime)
                return false;
            else
            {
                float value = JumpCurve.Evaluate(JumpTime / MaxJumpTime) * BounceHeight;
                Middleman.Machine.ApplyOrbitPosition(Vector3.up * value);

                JumpTime += Middleman.FDT;
                return true;
            }
        }
    }
}