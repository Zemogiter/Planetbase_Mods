using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deliveries.Patches
{
    /// <summary>
    /// This class disabled recycling until the ship has actually landed
    /// </summary>

    [HarmonyPatch(typeof(global::Planetbase.ColonyShip))]
    [HarmonyPatch("isDeleteable")]
    public class IsDeleteable
    {
        public static void Postfix(out bool buttonEnabled, ref bool __result)
        {
            __result = __result && Deliveries.Ship.isDone();
            buttonEnabled = __result;
        }
    }
}
