using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deliveries.Patches
{
    /// <summary>
    /// This class disables recycling until the ship has actually landed
    /// </summary>

    [HarmonyPatch(typeof(Planetbase.ColonyShip))]
    [HarmonyPatch("isDeleteable")]
    public class IsDeleteable
    {
        public static void Postfix(ref bool __result)
        {
            if (Deliveries.Ship != null)
            {
                __result = __result && Deliveries.Ship.isDone();
            }
        }
    }
}
