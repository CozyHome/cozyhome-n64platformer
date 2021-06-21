using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorEventRegistry : MonoBehaviour
{
    public Action Event_AnimatorMove;
    public Action<int> Event_AnimatorIK;

    void Start()
    {
        Event_AnimatorMove = null;
        Event_AnimatorIK = null;
    }
    void OnAnimatorMove() => Event_AnimatorMove?.Invoke();
    void OnAnimatorIK(int baselayer) => Event_AnimatorIK?.Invoke(baselayer);
}

[System.Serializable]
public class AnimationMoveBundle
{
    public Vector3 Displacement;
    public Quaternion Rotation;

    public void Clear() 
    {
        Displacement = Vector3.zero;
        Rotation = Quaternion.identity;
    }

    public void Accumulate(Vector3 delta_displacement, Quaternion delta_rotation) 
    {
        Displacement += delta_displacement;
        Rotation = delta_rotation * Rotation;
    }

    public Vector3 GetRootDisplacement(float fdt) => fdt > 0 ? Displacement / fdt : Vector3.zero; 
    public Quaternion GetRootRotation(float fdt) => Rotation; /* don't care much for super precise rotation here */
}