using System.Collections.Generic;
using UnityEngine;
using com.cozyhome.Archetype;


[RequireComponent(typeof(SphereCollider))]
public class OccludeRegistry : MonoBehaviour
{
    [Header("Occlusion Settings")]
    [SerializeField] private LayerMask ValidOcclusionMask;
    [SerializeField] private List<Collider> InvalidColliders;
    private ArchetypeHeader.SphereArchetype SphereArchetype;
    private SphereCollider SphereCollider;
    private RaycastHit[] InternalHits = new RaycastHit[5];


    void Start()
    {
        SphereCollider = GetComponent<SphereCollider>();
        SphereArchetype = new ArchetypeHeader.SphereArchetype(SphereCollider);
    }

    public float DetermineOcclusionRatio(Vector3 startposition, Vector3 displacement)
    {
        float tracelen = displacement.magnitude;

        if (tracelen <= 0F)
            return 1.0F;

        SphereArchetype.Trace(startposition,
            displacement / tracelen,
            tracelen,
            Quaternion.identity,
            ValidOcclusionMask,
            0F,
            QueryTriggerInteraction.Ignore,
            InternalHits,
            out int traces);

        ArchetypeHeader.TraceFilters.FindClosestFilterInvalidsList(ref traces, 
            out int i0, 
            0F, 
            InvalidColliders, 
            InternalHits);

        if(i0 < 0)
            return 1.0F;
        
        return (InternalHits[i0].distance / tracelen);
    }
}
