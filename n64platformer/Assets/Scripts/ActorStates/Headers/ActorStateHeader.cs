using System;
using UnityEngine;
public static class ActorStateHeader
{
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

            return CheckSwitchToLedge(Position, Machine, ledgehit);
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
