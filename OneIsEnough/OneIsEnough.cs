using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace OneIsEnough
{
    public class OneIsEnough : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new OneIsEnough(), modEntry, "OneIsEnough");

        public override void OnInitialized(ModEntry modEntry)
        {
            float[] singleNumber = new float[1] { 1f };
            ModuleName telescope = ModuleName.Telescope;
            typeof(ModuleTypeTelescope)
                .GetField("mDisasterInterceptionChances", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(telescope, singleNumber);
            ModuleName radio = ModuleName.RadioAntenna;
            typeof(ModuleTypeRadioAntenna)
                .GetField("mRadioInterceptionChances", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(radio, singleNumber);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
        }
    }
}
