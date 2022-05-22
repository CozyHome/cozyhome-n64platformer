using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    
	public BezierSpline spline;

	public float duration;

	private float progress;


    public SplineWalkerMode mode;

    public enum SplineWalkerMode {
	Once,
	Loop,
	PingPong
    }

	private bool goingForward = true;	
    public bool lookForward;

	private void Update () {
		if (goingForward) {
			progress += Time.deltaTime / duration;
			if (progress > 1f) {
				if (mode == SplineWalkerMode.Once) {
					progress = 1f;
				}
				else if (mode == SplineWalkerMode.Loop) {
					progress -= 1f;
				}
				else {
					progress = 2f - progress;
					goingForward = false;
				}
			}
		}
		else {
			progress -= Time.deltaTime / duration;
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}
		}

		Vector3 position = spline.GetPoint(progress);
		transform.localPosition = position;
		if (lookForward) 
        {
			transform.rotation = Quaternion.LookRotation(spline.GetDirection(progress), transform.up);
		}
	}
}
