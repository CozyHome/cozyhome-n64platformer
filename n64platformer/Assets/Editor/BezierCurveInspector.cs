using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor {

    private BezierCurve          curve;
    private Transform            handleTransform;
    private Quaternion           handleRotation;
    
    const int lineSteps     = 25;
    const float bezierScale = 1.5F;

    private void OnSceneGUI () 
    {

        curve = target as BezierCurve;

        handleTransform = curve.transform;
        handleRotation  = handleTransform.rotation;

        Vector3 p0 = ShowPoint(0);
		Vector3 p1 = ShowPoint(1);
		Vector3 p2 = ShowPoint(2);
		Vector3 p3 = ShowPoint(3);

        Handles.color = Color.red;
        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, bezierScale);
    }

    private void ShowDirections() 
    {
        Handles.color = Color.green;
        Vector3 p = curve.GetPoint(0F);
    
        Handles.DrawLine(p, p + curve.GetDirection(0F) * bezierScale);
        for(int i = 1;i <= lineSteps;i++)
        {
            p = curve.GetPoint(i / (float) lineSteps);
            Handles.DrawLine(p, p + bezierScale * curve.GetDirection(i / (float) lineSteps));
        }
    }

    private Vector3 ShowPoint(int index)
    {

        Vector3 point = handleTransform.TransformPoint(
            curve.points[index]
        );

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if(EditorGUI.EndChangeCheck()) 
        {   
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            curve.points[index] = handleTransform.InverseTransformPoint(point);
        }

        return point;
    }

}