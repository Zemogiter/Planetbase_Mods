using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace ModulesWithTanks
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Water storage for water-using modules (will double per size)")] public int waterStorageCapacity = 1000;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class ModulesWithTanks : ModBase
    {
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new ModulesWithTanks(), modEntry, "ModulesWithTanks");
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

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(Module), nameof(Module.update))]
    public class ModuleTankClass
    {
        static void Postfix(Module __instance)
        {
            var moduleList = BuildableUtils.GetAllModules();
            var waterUsersList = moduleList.Where(a => a.getWaterGeneration() < 0).ToList();
            foreach ( Module waterUser in waterUsersList ) 
            {
                var waterIndicator = waterUser.getIndicators().Where(a => a.getName().Equals(IndicatorType.WaterGrid)) as Indicator;
                waterIndicator.setOrientation(IndicatorOrientation.Horizontal);
                //waterIndicator.setLevels();
                
                if (waterUser.getSizeIndex() > 0)
                {
                    waterIndicator.setMax(ModulesWithTanks.settings.waterStorageCapacity * waterUser.getSizeIndex());
                }
                else
                {
                    waterIndicator.setOrientation(IndicatorOrientation.Horizontal);
                    waterIndicator.setMax(ModulesWithTanks.settings.waterStorageCapacity);
                }
            }
        }
    }
    /*
    [HarmonyPatch(typeof(ModuleType), nameof(ModuleType.getWaterStorageCapacity))]
    public class ModuleTypeTankClass
    {
        static void Postfix(ModuleType __instance)
        {
            var moduleTypeList = TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeList;
            var waterUsersList = moduleTypeList.Where(a => a.getWaterGeneration(0) < 0).ToList();
            foreach( ModuleType waterUser in waterUsersList )
            {
                waterUser.getWaterStorageCapacity() = ModulesWithTanks.settings.waterStorageCapacity;
            }
        }
    }
    */
}
