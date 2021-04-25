using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualOrbitState : CameraState
{

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve EasingCurve;

    [SerializeField] private float MaxEaseTime = 0.5F;
    private float EaseTime = 0F;


    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private float MaxOrbitSpeed = 240F;

    /* callbacks */
    [SerializeField] private OnJumpExecution JumpExecution;
    [SerializeField] private Middleman middleman;
    private ExecutionChain<int, Middleman> Chain;

    protected override void OnStateInitialize()
    {
        Chain = new ExecutionChain<int, Middleman>(middleman);

        machine.GetEventRegistry.Event_ActorJumped += delegate
        {
            Chain.AddExecution(JumpExecution);
        };

    }

    public override void Enter(CameraState prev)
    {

    }

    public override void Exit(CameraState next)
    {
        EaseTime = 0F;
        machine.ApplyOrbitPosition();
    }

    public override void FixedTick(float fdt)
    {
        bool LeftTrigger = PlayerInput.GetLeftTrigger;
        if (LeftTrigger)
        {
            machine.GetFSM.SwitchState("Align");
            return;
        }

        Vector2 Mouse = PlayerInput.GetRawMouse;

        if (Mouse.sqrMagnitude > 0F)
        {
            EaseTime += fdt;
            EaseTime = Mathf.Min(EaseTime, MaxEaseTime);
        }
        else
        {

            EaseTime -= fdt;
            EaseTime = Mathf.Max(EaseTime, 0F);
        }

        float rate = EasingCurve.Evaluate(EaseTime / MaxEaseTime);
        rate *= (MaxOrbitSpeed * fdt);

        machine.OrbitAroundTarget(Mouse * rate);
        machine.ApplyOrbitPosition();

        middleman.fdt = fdt;

        Chain.Tick();
    }

    public override void Tick(float dt)
    {

    }


    [System.Serializable]
    public class OnJumpExecution : ExecutionChain<int, Middleman>.Execution
    {
        [SerializeField] private AnimationCurve JumpCurve;
        [SerializeField] private float MaxJumpTime;
        private float JumpTime;

        public override void Enter(Middleman middleman) { JumpTime = 0F; }

        public override void Exit(Middleman middleman) { JumpTime = 0F; }

        public override bool Execute(Middleman middleman)
        {
            if (JumpTime >= MaxJumpTime)
                return false;
            else
            {
                middleman.machine.VerticalOffset = JumpCurve.Evaluate(JumpTime / MaxJumpTime);

                JumpTime += middleman.fdt;
                return true;
            }
        }
    }

    [System.Serializable]
    public class Middleman
    {
        public float fdt;
        public CameraMachine machine;
    }
}
