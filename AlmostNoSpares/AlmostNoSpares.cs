using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace AlmostNoSpares
{
    public class AlmostNoSpares : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new AlmostNoSpares(), modEntry, "AlmostNoSpares");

        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleName solar = ModuleName.SolarPanel;
            typeof(ModuleTypeSolarPanel)
                .GetField("mCondicionDecayTime", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(solar, 14000f);
            ModuleName wind = ModuleName.WindTurbine;
            typeof(ModuleTypeWindTurbine)
                .GetField("mCondicionDecayTime", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(wind, 14000f);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
}
