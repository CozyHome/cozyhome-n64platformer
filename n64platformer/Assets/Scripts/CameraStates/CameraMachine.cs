using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMachine : MonoBehaviour
{
    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private Transform OrbitTransform;
    [SerializeField] private Transform ViewTransform;
    [SerializeField] private float MaxVerticalAngle = 150F;
    [SerializeField] private float Sensitivity = 240F;
    [SerializeField] private float DolleyDistance = 16F;
    [SerializeField] private Vector3 WorldOffset;

    void FixedUpdate()
    {
        Vector2 Local = PlayerInput.GetRawMouse;

        if(Local.sqrMagnitude > 0F) 
        {
            ViewTransform.rotation = Quaternion.AngleAxis(
                Sensitivity * Local[0] * Time.fixedDeltaTime,
                Vector3.up
            ) * ViewTransform.rotation;

            float XAngle = Vector3.Angle(ViewTransform.forward, Vector3.up);
            float XDelta = Sensitivity * Local[1] * Time.fixedDeltaTime;

            if(XDelta + XAngle > MaxVerticalAngle)
                XDelta = MaxVerticalAngle - XAngle;
            else if(XDelta + XAngle < 180F - MaxVerticalAngle)
                XDelta = (180F - MaxVerticalAngle) - XAngle;
            
            ViewTransform.rotation = Quaternion.AngleAxis(
                XDelta,
                ViewTransform.right
            ) * ViewTransform.rotation;
        }

        ViewTransform.position = WorldOffset + OrbitTransform.position - ViewTransform.forward * DolleyDistance;
    }
}

/*
        [SerializeField] private float Sensitivity = 360F;
        [SerializeField] private PlayerInput PlayerInput;
        [SerializeField] private Transform OrbitTransform;
        [SerializeField] private Transform ViewTransform;
        [SerializeField] private Vector3 WorldOffset;

        ViewTransform.rotation = Quaternion.AngleAxis(
            Sensitivity * PlayerInput.GetRawMouse[0] * Time.fixedDeltaTime, 
            Vector3.up) * ViewTransform.rotation;
        
        float XAngle = Vector3.Angle(ViewTransform.forward, Vector3.up);

        float XDelta = PlayerInput.GetRawMouse[1] * Sensitivity * Time.fixedDeltaTime;

        if(XAngle + XDelta > 180F)
            XDelta = 180F - XAngle;
        else if(XAngle + XDelta < 0F)
            XDelta = 0F - XAngle;

        ViewTransform.rotation = Quaternion.AngleAxis(
            XDelta, 
            ViewTransform.right) * ViewTransform.rotation;
        
        ViewTransform.position = WorldOffset + OrbitTransform.position - ViewTransform.forward * 10F;
*/