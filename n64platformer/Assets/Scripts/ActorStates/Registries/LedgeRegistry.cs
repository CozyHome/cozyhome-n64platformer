using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Actors;
using com.cozyhome.Archetype;
using com.cozyhome.Vectors;
using UnityEngine;


public class LedgeRegistry : MonoBehaviour
{
    private const int HIT_PRIMIVITE_LEDGETRACE = 0x0001;
    private const int BLOCKING_PRIMITIVE_LEDGETRACE = 0x0002;
    private const int VALID_LINE_LEDGETRACE = 0x0004;
    private const int SAFE_PRIMITIVE_LEDGETRACE = 0x0008;


    public struct LedgeHit
    {
        /*
        0th/X Component - Magnitude of displacement to ledge position planar to the upward vector
        1th/Y Component - Magnitude of displacement to ledge position along the upward vector
        */
        public Vector2 LedgeDelta;
        public Vector3 LedgePlanarNormal;
        public Vector3 LedgeUpwardNormal;
        public Vector3 AuxillaryDelta;

        public int LedgeStatus;

        public void Clear()
        {
            LedgeDelta = Vector2.zero; // reset deltas 
            LedgeStatus &= ~(LedgeStatus); // CLR status bits
            LedgePlanarNormal = Vector3.zero; // clear normals
            LedgeUpwardNormal = Vector3.zero; // clear normals
        }

        public Vector3 Ledge_LocalToWorldDelta()
        {
            return (LedgeDelta[0] * LedgePlanarNormal) + (LedgeDelta[1] * LedgeUpwardNormal);
        }

        public Vector3 Auxillary_LocalToWorldDelta()
        {
            return (AuxillaryDelta[0] * LedgePlanarNormal) + (AuxillaryDelta[1] * LedgeUpwardNormal);
        }

        public bool IsHit => (LedgeStatus & HIT_PRIMIVITE_LEDGETRACE) != 0;
        public bool IsBlocking => (LedgeStatus & BLOCKING_PRIMITIVE_LEDGETRACE) != 0;
        public bool IsLedge => (LedgeStatus & VALID_LINE_LEDGETRACE) != 0;
        public bool IsSafe => (LedgeStatus & SAFE_PRIMITIVE_LEDGETRACE) != 0;
    }

    public enum LedgeResult
    {
        FoundInvalidLedge = 0,
        FoundValidLedge = 1,
    };

    [Header("Ledge Registry References")]
    private RaycastHit[] InternalHits = new RaycastHit[5];
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

    public void DetectLedge(
        Vector3 position,
        Vector3 forward,
        Quaternion orientation,
        float dist,
        out LedgeHit ledgehit)
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

        ledgehit = new LedgeHit();
        ledgehit.Clear();

        /* trace from player */
        Archetype.Trace(
            position,
            forward,
            dist,
            orientation,
            ValidLedgeMask,
            0F,
            QueryTriggerInteraction.Ignore,
            InternalHits,
            out int traces);

        ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
            ref traces,
            out int i0,
            ArchetypeHeader.GET_TRACEBIAS(ArchetypeHeader.ARCHETYPE_BOX),
            Archetype.Collider(),
            InternalHits);

        if (i0 >= 0)
        {
            /* Valid Primitive Trace */
            ledgehit.LedgeStatus |= HIT_PRIMIVITE_LEDGETRACE;
            ValidateLedge(
                        InternalHits[i0].distance,
                        InternalHits[i0].normal,
                        InternalHits[i0].point,
                        position,
                        orientation,
                        ref ledgehit);
        }
    }

    public void ValidateLedge(
        float dist,
        Vector3 normal,
        Vector3 point, // referring to trace point
        Vector3 position, // referring to player position
        Quaternion orientation,
        ref LedgeHit ledgehit)
    {
        const float min_hoffset = 0.1F, min_voffset = 0.1F, min_correlation = 0.025F;

        /* is our primitive trace really hitting a wall, or is it a ceiling/slope? */
        if (VectorHeader.Dot(normal, Vector3.up) < min_correlation)
        {
            ledgehit.AuxillaryDelta[0] = -dist;
            ledgehit.LedgePlanarNormal = normal;

            /* Obstruction in Primitive Trace */
            ledgehit.LedgeStatus |= BLOCKING_PRIMITIVE_LEDGETRACE;

            float fdot = VectorHeader.Dot((point - position), normal);

            Vector3 aux = position;
            Vector3 up = orientation * Vector3.up;

            /* consider our primitive's local offset */
            aux += orientation * Archetype.Center();
            aux -= up * Archetype.Height() / 2F;

            /* move our query position into the infinite plane and upwards for our downward line trace */
            aux += normal * (fdot - min_hoffset);
            aux += up * (MaxLedgeHeight + min_voffset);

            /* downward line trace validation */
            int traces = ArchetypeHeader.TraceRay(aux,
                -up,
                MaxLedgeHeight + min_voffset,
                InternalHits,
                ValidLedgeMask);

            ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
                ref traces,
                out int i0,
                ArchetypeHeader.GET_TRACEBIAS(ArchetypeHeader.ARCHETYPE_LINE),
                Archetype.Collider(),
                InternalHits);

            /* did our line trace anything? */
            if (i0 >= 0)
            {
                RaycastHit floorinfo = InternalHits[i0];
                float height = (MaxLedgeHeight + min_voffset) - floorinfo.distance;

                ledgehit.LedgeStatus |= VALID_LINE_LEDGETRACE;

                ledgehit.LedgeDelta[0] = -(min_hoffset) + ledgehit.AuxillaryDelta[0];
                ledgehit.LedgeDelta[1] = (height + min_voffset);

                ledgehit.LedgeUpwardNormal = floorinfo.normal;

                /* determining if our floor hit is tall enough to constitute a ledge. */
                /* determining if our floor hit is actually stable/perpendicular to us. */
                if (height >= MinLedgeHeight &&
                    VectorHeader.Dot(ledgehit.LedgeUpwardNormal, up) >= (1 - min_correlation))
                {
                    /* overlap safety check */
                    
                    /* inflate */

                    Archetype.Overlap(
                        position + ledgehit.Ledge_LocalToWorldDelta() + up * (0.05F),
                        orientation,
                        ValidLedgeMask,
                        0.05F,
                        QueryTriggerInteraction.Ignore,
                        InternalOverlaps,
                        out int overlaps);

                    /* filter invalid colliders from our query search */
                    ArchetypeHeader.OverlapFilters.FilterSelf(ref overlaps,
                        Archetype.Collider(),
                        InternalOverlaps);

                    /* end of execution */
                    ledgehit.LedgeStatus |= (overlaps > 0) ? 0 : SAFE_PRIMITIVE_LEDGETRACE;
                }
            }
        }
    }

    public float GetProbeDistance => ProbeDistance;
}
