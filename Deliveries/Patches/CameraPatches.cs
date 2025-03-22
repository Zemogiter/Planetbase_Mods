using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deliveries.Patches
{
    /// <summary>
    /// This class causes the camera to NOT focus on the landing ship during deliveries.
    /// </summary>

    [HarmonyPatch(typeof(global::Planetbase.CameraManager))]
    [HarmonyPatch("setCinematic")]
    public class SetCinematic
    {
        public static bool Prefix(Cinematic cimenatic)
        {
            //Skip intro/landing camera cinematics only if a delivery has been called.
            if (cimenatic is IntroCinemetic)
                return !Deliveries.ActiveDeliveryShip;

            return true;
        }
    }
}
