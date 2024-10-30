using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace BetterLandingPermissions
{
    public class BetterLandingPermissions : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new BetterLandingPermissions(), modEntry, "BetterLandingPermissions");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(GuiLandingPermissions), nameof(GuiLandingPermissions.update))]
    public class StatsWindowPatch
    {
        public static bool Prefix()
        {
            
            return false;
        }
    }
}
