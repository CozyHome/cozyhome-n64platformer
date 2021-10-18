using com.cozyhome.ChainedExecutions;
using com.cozyhome.Actors;
using com.cozyhome.Timers;
using com.cozyhome.Vectors;
using UnityEngine;

public partial class ExecutionHeader
{
    public static class Panels
    {
        public class WrappedPanel
        {
            public float DT() => Time.deltaTime;
        }

        public class OnToggleExecution : ExecutionChain<int, WrappedPanel>.Execution
        {
            public OnToggleExecution(int key) : base(key) { }

            private RectTransform          UIPanel;
            private AnimationCurve         EaseCurve;
            private Vector2                Start;
            private Vector2                End;
            private TimerHeader.DeltaTimer Timer;
            // private Action OnExecutionFinished();

            public void Assign(
                float          Duration,
                RectTransform  UIPanel,
                AnimationCurve EaseCurve, 
                Vector2        Start, 
                Vector2        End)
            {
                this.UIPanel   = UIPanel;
                this.EaseCurve = EaseCurve;
                this.Start     = Start;
                this.End       = End;

                Timer.Max(Duration);
            }

            public override void Enter(WrappedPanel WrappedPanel) => Timer.Reset();

            public override ExecutionState Execute(WrappedPanel WrappedPanel)
            {
                Timer.Accumulate(WrappedPanel.DT());

                float elapsed = Timer.NormalizedElapsed;
                float curve   = EaseCurve.Evaluate(elapsed);

                Vector2 Theta = Start + (End - Start) * curve;
                UIPanel.position = Theta;

                return Timer.Check() 
                    ? ExecutionState.FINISHED
                    : ExecutionState.ACTIVE;
            }

            public override void Exit(WrappedPanel WrappedPanel) => Timer.Reset(); // OnExecutionFinished( );
            
        }
    }
}