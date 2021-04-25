using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum OrbitType
{
    Automatic = 0,
    Manual = 1,
    Realign = 2
}

public class OrbitState : CameraState
{
    [Header("Outer References")]
    [SerializeField] private PlayerInput PlayerInput;
    [Header("Target References")]
    [SerializeField] private Transform OrbitTransform;
    [SerializeField] private Transform ViewTransform;

    [Header("General Values")]
    [SerializeField] private OrbitType OrbitType;
    [SerializeField] private float MaxVerticalAngle = 150F;
    [SerializeField] private float ManualSensitivity = 240F;
    [SerializeField] private float AutomaticSensitivity = 160F;
    [SerializeField] private float DolleyDistance = 16F;

    [Header("Offset Values")]
    [SerializeField] private Vector3 WorldOffset;

    private bool RealignInitialized = false;
    private float RealignTime = 0F;
    private Quaternion FinalRealignedRotation = Quaternion.identity, InitialRealignedRotation;
    [SerializeField] private AnimationCurve RealignCurve;
    [SerializeField] private float RealignDuration = 0.75F;

    protected override void OnInitialize()
    {
        //machine.SetCurrentState(this.GetKey);
    }

    public override void Enter(string previous_key, CameraState previous_state)
    {

    }

    public override void Exit(string next_key, CameraState next_state)
    {

    }

    public override void FixedTick(float fdt)
    {
        Vector2 LocalMouse = PlayerInput.GetRawMouse;

        Vector2 LocalMove = PlayerInput.GetRawMove;
        LocalMove[1] = 0F;

        bool LeftTrigger = PlayerInput.GetLeftTrigger;

        if (LeftTrigger)
            OrbitType = OrbitType.Realign;

        if (OrbitType == OrbitType.Realign)
        {
            if (RealignInitialized)
            {
                float et = Time.time - RealignTime;
                if (et >= RealignDuration)
                {
                    ViewTransform.rotation = FinalRealignedRotation;
                    OrbitType = OrbitType.Automatic;
                    RealignInitialized = false;
                }
                else
                {
                    float b = RealignCurve.Evaluate(et / RealignDuration);
                    ViewTransform.rotation = Quaternion.Lerp(
                        InitialRealignedRotation,
                        FinalRealignedRotation,
                        b);
                }

            }
            else
            {
                Vector3 planarforward = ViewTransform.forward;
                planarforward[1] = 0;
                planarforward.Normalize();

                float YAngle = Vector3.SignedAngle(planarforward, OrbitTransform.forward, Vector3.up);
                FinalRealignedRotation = Quaternion.AngleAxis(YAngle, Vector3.up) * ViewTransform.rotation;
                InitialRealignedRotation = ViewTransform.rotation;

                RealignInitialized = true;
                RealignTime = Time.time;
            }
        }
        else
        {
            if (OrbitType == OrbitType.Automatic)
            {
                if (LocalMouse.sqrMagnitude > 0F)
                    OrbitType = OrbitType.Manual;
                else if (LocalMove.sqrMagnitude > 0F)
                    machine.OrbitAroundTarget(LocalMove * AutomaticSensitivity * fdt);
            }

            /* if player chooses to move with mouse in automatic it will be updated here */

            if (OrbitType == OrbitType.Manual)
            {
                if (LocalMouse.sqrMagnitude > 0F)
                    machine.OrbitAroundTarget(LocalMouse * ManualSensitivity * fdt);
            }
        }

        ViewTransform.position = WorldOffset + OrbitTransform.position - ViewTransform.forward * DolleyDistance;
    }



    public override void Tick(float dt)
    {

    }

}
