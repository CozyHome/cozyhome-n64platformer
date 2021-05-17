using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Vectors;
using UnityEngine;

public static class ExecutionHeader
{
    public static class Camera
    {

        [System.Serializable]
        public class OnJumpExecution : ExecutionChain<int, CameraMiddleman>.Execution
        {
            [SerializeField] private AnimationCurve EaseCurve;
            [SerializeField] private float MaxJumpTime;
            [SerializeField] private float BounceHeight = 2F;
            private float JumpTime;

            public override void Enter(CameraMiddleman Middleman) { JumpTime = 0F; }

            public override void Exit(CameraMiddleman Middleman) { JumpTime = 0F; }

            public override bool Execute(CameraMiddleman Middleman)
            {
                if (JumpTime > MaxJumpTime)
                    return false;
                else
                {
                    float value = EaseCurve.Evaluate(JumpTime / MaxJumpTime) * BounceHeight;
                    Middleman.GetMachine.ApplyOffset(Vector3.up * value);

                    JumpTime += Middleman.FDT;
                    return true;
                }
            }
        }

        [System.Serializable]
        public class OnHangExecution : ExecutionChain<int, CameraMiddleman>.Execution
        {
            [SerializeField] private AnimationCurve EaseCurve;
            [SerializeField] private Vector3 InitialPosition, HangPosition;
            [SerializeField] private float MaxHangTime = 1.0F;
            private float HangTime = 0.0F;
            private bool EntryNotSet;

            public override void Enter(CameraMiddleman Middleman)
            {
                HangTime = 0.0F;
                EntryNotSet = true;
            }
            public override void Exit(CameraMiddleman Middleman)
            {
                HangTime = 0.0F;
            }

            public override bool Execute(CameraMiddleman Middleman)
            {
                if (HangTime > MaxHangTime)
                    return false;
                else
                {
                    if (EntryNotSet)
                    {
                        InitialPosition = Middleman.GetMachine.ViewPosition;
                        EntryNotSet = false;
                    }

                    float Amount = EaseCurve.Evaluate(HangTime / MaxHangTime);
                    Middleman.GetMachine.SetViewPosition(
                        Vector3.Lerp(InitialPosition,
                            Middleman.GetMachine.ViewPosition,
                            Amount));

                    HangTime += Middleman.FDT;
                    return true;
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
            [SerializeField] private float MaxLedgeTime;

            private float LedgeTime;
            private Vector3 ledge_position, hang_position, mantle_position;
            private Quaternion hang_rotation, ledge_rotation;
            public override void Enter(ActorMiddleman Middleman)
            {
                /* when we are activated, we will be able to do something cool */
                LedgeTime = 0F;
            }
            public override void Exit(ActorMiddleman Middleman)
            {
                LedgeTime = 0F;
            }

            public override bool Execute(ActorMiddleman Middleman)
            {
                /* when we are run after Move(), assign our actor position to the required position, and rotation */
                /* if anything, since we have an update loop to work with, we could potentially add easing to this */
                ActorHeader.Actor Actor = Middleman.Machine.GetActor;
                Transform ModelView = Middleman.Machine.GetModelView;

                if (LedgeTime > MaxLedgeTime)
                    return false;
                else
                {
                    LedgeTime += Middleman.FDT;

                    float percent = LedgeCurve.Evaluate(LedgeTime / MaxLedgeTime);

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

                    return true;
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
                this.mantle_position = mantle_position;

                this.ledge_rotation = ledge_rotation;

                this.hang_rotation = Quaternion.LookRotation(
                    VectorHeader.ClipVector(mantle_position - ledge_position, Vector3.up),
                    Vector3.up);
            }
        }
    }
}