using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor 
{
	
    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

	private const int lineSteps = 10;
	private const float directionScale = 0.5f;

	private BezierSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;

    private void OnSceneGUI() 
    {
        spline = target as BezierSpline;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
        handleTransform.rotation : Quaternion.identity;
		
		Handles.color = Color.gray;
		
		Vector3 p0 = ShowPoint(0);
		for(int i = 1;i < spline.ControlPointCount;i += 3)
		{
			Vector3 p1 = ShowPoint(i);
			Vector3 p2 = ShowPoint(i + 1);
			Vector3 p3 = ShowPoint(i + 2);
			
			Handles.color = Color.grey;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p2);

			Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 4f);
			p0 = p3;
		}
		
		ShowDirections();
    }

    public override void OnInspectorGUI() 
    {
        // DrawDefaultInspector();
        spline = target as BezierSpline;
        

		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.Loop = loop;
		}

		if(selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
			DrawSelectedPointInspector();

		if(GUILayout.Button("Add Curve"))
		{
			Undo.RecordObject(spline, "Add Curve");
			spline.AddCurve();
			EditorUtility.SetDirty(spline);
		}
    }

    private void DrawSelectedPointInspector()
    {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
		}

		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)
			EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(spline, "Change Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
		}
	}

    private void ShowDirections () 
    {
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0f);
		Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
		int steps = lineSteps * spline.CurveCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint(i / (float)steps);
			Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
		}
	}

	private const float handleSize = 0.04F;
	private const float pickSize   = 0.06F;

	private int selectedIndex = -1;

	private Vector3 ShowPoint(int index)
	{
		Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
		Handles.color = modeColors[(int)spline.GetControlPointMode(index)];

		float size = HandleUtility.GetHandleSize(point);

		if(index == 0)
			size *= 2F;

		if(Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
		{
			selectedIndex = index;
			Repaint(); // issue redraw once a new index is selected
		}
		if(selectedIndex == index)
		{
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
			}
		}

		return point;
	}

}