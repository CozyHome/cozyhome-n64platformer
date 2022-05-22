using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Vector3[] points;

    public void Reset() 
    {
        points = new Vector3[] {
            new Vector3(1F, 0F, 0F),
            new Vector3(2F, 0F, 0F),
            new Vector3(3F, 0F, 0F),
            new Vector3(4F, 0F, 0F),
        };
    }

    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(
            Bezier.GetPoint(points[0], points[1], points[2], points[3], t)
        );
    }

    public Vector3 GetVelocity(float t) 
    {
        return transform.TransformVector(
            Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t));
    }
    
    public Vector3 GetDirection(float t) 
    {
        return transform.TransformDirection(
            Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)).normalized;
    }
}
