using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using static Planetbase.ModuleType;

namespace PanelsWithoutModules
{
    public class PanelsWithoutModules : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new PanelsWithoutModules(), modEntry, "PanelsWithoutModules");

        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleName control = ModuleName.ControlCenter;
            typeof(ModuleTypeControlCenter).GetField("mRelatedPanel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(control, Panel.Invalid);
            ModuleName landing = ModuleName.LandingPad;
            typeof(ModuleTypeLandingPad).GetField("mRelatedPanel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(landing, Panel.Invalid);
            ModuleName factory = ModuleName.Factory;
            typeof(ModuleTypeFactory).GetField("mRelatedPanel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(factory, Panel.Invalid);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing required here for now
        }
    }
}
