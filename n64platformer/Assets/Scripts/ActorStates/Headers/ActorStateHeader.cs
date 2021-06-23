using System;
using com.cozyhome.Vectors;
using UnityEngine;
public static class ActorStateHeader
{
    public static Vector3 ComputeMoveVector(Vector2 Local, Quaternion CameraOrientation, Vector3 ModelUp)
    {
        return VectorHeader.CrossProjection(
            CameraOrientation * new Vector3(Local[0], 0F, Local[1]),
            CameraOrientation * Vector3.up,
            ModelUp);
    }

    public static void AccumulateDeviatingGravity(ref Vector3 Velocity, float ratio, float fdt, float gravitational_pull)
        => Velocity[1] -= (fdt * gravitational_pull * ratio);

    public static void AccumulateConstantGravity(ref Vector3 Velocity, float fdt, float gravitational_pull)
        => Velocity[1] -= (fdt * gravitational_pull);

    public static void RepairTime(
        float FDT,
        float Turn,
        float TurnSpeed,
        float HorizontalSpeed,
        float Influence,
        Vector3 Move,
        Transform ModelView,
        ref Vector3 Velocity)
    {
        /* Rotate Towards */
        if (Move.sqrMagnitude > 0F)
        {
            ModelView.rotation = Quaternion.RotateTowards(
                ModelView.rotation,
                Quaternion.LookRotation(Move, Vector3.up),
                Turn * TurnSpeed * FDT);

            Vector3 HorizontalV = Vector3.Scale(Velocity, new Vector3(1F, 0F, 1F));

            Velocity -= HorizontalV;
            HorizontalV += Move * (Influence * Turn * FDT);
            HorizontalV = Vector3.ClampMagnitude(HorizontalV, HorizontalSpeed);
            Velocity += HorizontalV;
        }
    }

    public static class Transitions
    {
        public static bool CheckGeneralLedgeTransition(
            Vector3 Position,
            Vector3 Forward,
            Quaternion Orientation,
            LedgeRegistry LedgeRegistry,
            PlayerMachine Machine)
        {
            LedgeRegistry.DetectLedge(
                Position,
                Forward,
                Orientation,
                LedgeRegistry.GetProbeDistance,
                out LedgeRegistry.LedgeHit ledgehit);

            return CheckSwitchToLedge(Position, Machine, ledgehit) || CheckSwitchToSlide(Position, Machine, ledgehit);
        }

        public static bool CheckSlideTransitions(Vector3 Position,
            Vector3 Forward,
            Quaternion Orientation,
            LedgeRegistry LedgeRegistry,
            PlayerMachine Machine)
        {
            LedgeRegistry.DetectLedge(
                Position,
                Forward,
                Orientation,
                LedgeRegistry.GetProbeDistance,
                out LedgeRegistry.LedgeHit ledgehit);

            return CheckSwitchToLedge(Position, Machine, ledgehit) ||
                    CheckSwitchToFall(Position, Machine, ledgehit);
        }


        private static bool CheckSwitchToLedge(Vector3 Position, PlayerMachine Machine, LedgeRegistry.LedgeHit ledgehit)
        {
            if (ledgehit.IsSafe)
            {
                Machine.GetFSM.SwitchState(
                    (ActorState next) =>
                    {
                        ((LedgeState)next).Prepare(Position + ledgehit.Auxillary_LocalToWorldDelta(),
                                                   Position + ledgehit.Ledge_LocalToWorldDelta());
                    }, "Ledge");

                return true;
            }
            else
                return false;
        }

        private static bool CheckSwitchToSlide(Vector3 Position, PlayerMachine Machine, LedgeRegistry.LedgeHit ledgehit)
        {
            bool IsBlockingWall = ledgehit.AuxillaryDelta[0] >= -0.125F && (ledgehit.IsBlocking) && !(ledgehit.IsLedge);

            if (IsBlockingWall && (ledgehit.LedgeDelta[1] == 0F || ledgehit.LedgeDelta[1] >= 4F))
            {
                Machine.GetFSM.SwitchState(
                    (ActorState next) =>
                    {
                        ((WallSlideState)next).Prepare(ledgehit.LedgePlanarNormal, Machine.GetModelView.forward);
                    }, "WallSlide");

                return true;
            }
            else
                return false;
        }

        private static bool CheckSwitchToFall(Vector3 Position, PlayerMachine Machine, LedgeRegistry.LedgeHit ledgehit)
        {
            if (!ledgehit.IsHit)
            {
                Machine.GetFSM.SwitchState("Fall");
                return true;
            }
            else
                return false;
        }
    }
}
