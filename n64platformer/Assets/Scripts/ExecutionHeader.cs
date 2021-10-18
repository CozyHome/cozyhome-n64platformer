using com.cozyhome.Actors;
using com.cozyhome.Timers;
using com.cozyhome.Vectors;

using com.cozyhome.ChainedExecutions;

using UnityEngine;

public partial class ExecutionHeader
{
    public static class Camera
    {

        [System.Serializable]
        public class OnJumpExecution : ExecutionChain<int, CameraMiddleman>.Execution
        {
            [SerializeField] private AnimationCurve EaseCurve;
            [SerializeField] private TimerHeader.DeltaTimer JumpTimer;
            [SerializeField] private float BounceHeight = 2F;

            public override void Enter(CameraMiddleman Middleman) { }

            public override void Exit(CameraMiddleman Middleman) { JumpTimer.Reset(); }

            public override ExecutionState Execute(CameraMiddleman Middleman)
            {
                if (JumpTimer.Check())
                    return ExecutionState.FINISHED;
                else
                {
                    float value = EaseCurve.Evaluate(JumpTimer.NormalizedElapsed) * BounceHeight;

                    Middleman.GetMachine.ApplyOffset(Vector3.up * value);
                    JumpTimer.Accumulate(Middleman.FDT);
                    return ExecutionState.ACTIVE;
                }
            }
        }

        [System.Serializable]
        public class OnHangExecution : ExecutionChain<int, CameraMiddleman>.Execution
        {
            [SerializeField] private AnimationCurve EaseCurve;
            [SerializeField] private Vector3 InitialPosition;
            [SerializeField] private TimerHeader.DeltaTimer HangTimer;

            public override void Enter(CameraMiddleman Middleman)
            {
                InitialPosition = Middleman.GetMachine.ViewPosition;
                HangTimer.Reset();
            }
            public override void Exit(CameraMiddleman Middleman) { }

            public override ExecutionState Execute(CameraMiddleman Middleman)
            {
                if (HangTimer.Check())
                    return ExecutionState.FINISHED;
                else
                {
                    float Amount = EaseCurve.Evaluate(HangTimer.NormalizedElapsed);

                    InitialPosition = Vector3.Lerp(
                        InitialPosition,
                        Middleman.GetMachine.ViewPosition,
                        Amount);

                    Middleman.GetMachine.SetViewPosition(InitialPosition);

                    HangTimer.Accumulate(Middleman.FDT);
                    return ExecutionState.ACTIVE;
                }
            }
        }

    }

    public static class Actor
    {
        public enum ExecutionIndex
        {
            OnLedgeExecution = 0
        };

        [System.Serializable]
        public class OnLedgeExecution : ExecutionChain<ExecutionIndex, ActorMiddleman>.Execution
        {
            [SerializeField] private AnimationCurve LedgeCurve;

            [SerializeField] private TimerHeader.DeltaTimer LedgeTimer;
            private Vector3 ledge_position, hang_position;
            private Quaternion hang_rotation, ledge_rotation;
            public override void Enter(ActorMiddleman Middleman)
            {
                /* when we are activated, we will be able to do something cool */
                LedgeTimer.Reset();
            }
            public override void Exit(ActorMiddleman Middleman) { }

            public override ExecutionState Execute(ActorMiddleman Middleman)
            {
                /* when we are run after Move(), assign our actor position to the required position, and rotation */
                /* if anything, since we have an update loop to work with, we could potentially add easing to this */
                ActorHeader.Actor Actor = Middleman.Machine.GetActor;
                Transform ModelView = Middleman.Machine.GetModelView;

                if (LedgeTimer.Check())
                    return ExecutionState.FINISHED;
                else
                {
                    float percent = LedgeCurve.Evaluate(LedgeTimer.NormalizedElapsed);

                    Actor.SetPosition(
                        Vector3.Lerp(
                            ledge_position,
                            hang_position,
                            percent)
                        );

                    ModelView.rotation =
                        Quaternion.Slerp(
                            ledge_rotation,
                            hang_rotation,
                            percent);

                    Middleman.Machine.GetActor.SetVelocity(Vector3.zero);

                    LedgeTimer.Accumulate(Middleman.FDT);
                    return ExecutionState.ACTIVE;
                }
            }

            public void Prepare(
                Vector3 ledge_position,
                Vector3 hang_position,
                Vector3 mantle_position,
                Quaternion ledge_rotation)
            {
                this.ledge_position = ledge_position;
                this.hang_position = hang_position;

                this.ledge_rotation = ledge_rotation;

                this.hang_rotation = Quaternion.LookRotation(
                    VectorHeader.ClipVector(mantle_position - ledge_position, Vector3.up),
                    Vector3.up);
            }
        }
    }
}