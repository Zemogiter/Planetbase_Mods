using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using UnityModManagerNet;

namespace AlmostNoSpares
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("New decay time for solar panels")] public float solarPanelDecayTime = 14000f;
        [Draw("New decay time for wind turbines")] public float windTurbineDeacyTime = 14000f;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class AlmostNoSpares : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new AlmostNoSpares(), modEntry, "AlmostNoSpares");
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleName solar = ModuleName.SolarPanel;
            typeof(ModuleTypeSolarPanel)
                .GetField("mCondicionDecayTime", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(solar, settings.solarPanelDecayTime);
            ModuleName wind = ModuleName.WindTurbine;
            typeof(ModuleTypeWindTurbine)
                .GetField("mCondicionDecayTime", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(wind, settings.windTurbineDeacyTime);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
}
