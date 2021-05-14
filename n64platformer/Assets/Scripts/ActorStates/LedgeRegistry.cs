using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Archetype;
using com.cozyhome.Vectors;
using UnityEngine;


public class LedgeRegistry : MonoBehaviour
{
    enum LedgeDetectionState 
    {
        FoundObstruction = 0,
        FoundUnstableLedge = 1,
        FoundStableLedge = 2
    };

    [Header("Ledge Registry References")]
    private RaycastHit[] Internalhits = new RaycastHit[5];
    private Collider[] InternalOverlaps = new Collider[5];
    private ArchetypeHeader.Archetype Archetype;
    [Header("Ledge Registry Values")]
    [SerializeField] private float MaxLedgeHeight = 5.0F;
    [SerializeField] private float MinLedgeHeight = 0.5F;
    [SerializeField] private float ProbeDistance = 0.05F;
    [SerializeField] private LayerMask ValidLedgeMask;

    void Start()
    {
        Archetype = GetComponent<ActorHeader.Actor>().GetArchetype();
    }

    public bool DetectLedge(
        float dist,
        Vector3 position,
        Vector3 forward,
        Quaternion orientation,
        out Vector3 ledge_position)
    {
        /* 
        Ledge Algorithm:
        (1) Trace player forwards into obstruction
        (2) Trace auxillary line downward inside the bounds of the obstruction's infinite plane
        (3) If hit point determined:
            compute height difference from player feet to that of the hit point (dot product)
            iff height difference is greater than minimum requirements, the step/ledge is valid
            height difference will always <= MaxLedgeHeight as the linecast offsets from player feet
        (4) return the new ledge position to caller
        */

        ledge_position = position;
        /* trace from player */
        Archetype.Trace(
            position,
            forward,
            dist,
            orientation,
            ValidLedgeMask,
            0F,
            QueryTriggerInteraction.Ignore,
            Internalhits,
            out int traces);

        ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
            ref traces,
            out int i0,
            ArchetypeHeader.GET_TRACEBIAS(ArchetypeHeader.ARCHETYPE_BOX),
            Archetype.Collider(),
            Internalhits);

        if (i0 < 0)
            return false;

        return ValidateLedge(
            Internalhits[i0].normal,
            Internalhits[i0].point,
            position,
            orientation,
            out ledge_position);
    }

    public bool ValidateLedge(
        Vector3 normal,
        Vector3 point, // referring to trace point
        Vector3 position, // referring to player position
        Quaternion orientation,
        out Vector3 ledge_position)
    {
        const float min_hoffset = 0.1F, min_voffset = 0.1F;

        ledge_position = position;
        /*
            step one: linecast down
            step two: overlap
            step three: profit
        */

        if (VectorHeader.Dot(normal, Vector3.up) > 0.1F)
            return false;

        Vector3 aux = position;
        Vector3 up = orientation * Vector3.up;
        aux += orientation * Archetype.Center();
        aux -= up * Archetype.Height() / 2F;

        float fdot = VectorHeader.Dot((point - position), normal);

        aux += normal * (fdot - min_hoffset);

        aux += up * (MaxLedgeHeight + min_voffset);

        /* line trace / ray trace */

        int traces = ArchetypeHeader.TraceRay(aux,
            -up,
            MaxLedgeHeight + min_voffset,
            Internalhits,
            ValidLedgeMask);

        ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
            ref traces,
            out int i0,
            ArchetypeHeader.GET_TRACEBIAS(ArchetypeHeader.ARCHETYPE_LINE),
            Archetype.Collider(),
            Internalhits);

        if (i0 < 0)
            return false;

        RaycastHit floorinfo = Internalhits[i0];

        if (VectorHeader.Dot(floorinfo.normal, up) < 0.95F)
            return false;

        float height = (MaxLedgeHeight + min_voffset) - floorinfo.distance;

        if (height < MinLedgeHeight)
            return false;

        ledge_position += up * (height + min_voffset);
        ledge_position -= normal * (min_hoffset);

        /* overlap safety check */

        Archetype.Overlap(
            ledge_position,
            orientation,
            ValidLedgeMask,
            0F,
            QueryTriggerInteraction.Ignore,
            InternalOverlaps,
            out int overlaps);

        ArchetypeHeader.OverlapFilters.FilterSelf(ref overlaps,
            Archetype.Collider(),
            InternalOverlaps);

        if (overlaps > 0)
            return false;

        return true;
    }

    public float GetProbeDistance => ProbeDistance;
}
