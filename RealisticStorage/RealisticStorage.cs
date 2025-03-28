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

namespace RealisticStorage
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Hardcore mode(destroys food if storage module is damaged)")] public bool hardcoreMode = true;
        [Draw("Debug mode")] public bool debugMode = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class RealisticStorage : ModBase
    {
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new RealisticStorage(), modEntry, "RealisticStorage");
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
    public class StorageModulePatch
    {
        static void Postfix(Module __instance)
        {
            var moduleList = BuildableUtils.GetAllModules();
            var storageModuleList = moduleList.Where(module => module.getModuleType() is ModuleTypeStorage);

            foreach ( Module module in storageModuleList ) 
            {
                if(module.isExtremelyDamaged()) //if storage module is hit by meteor or lighting
                {
                    var storedResources = module.GetResourceStorageObject();
                    //gets slots from storage object, generates list of stored resources and then changes the state for each element of that list
                    //To-do: test this
                    storedResources.GetSlots().ForEach(slot => slot.GetResources().ToList().ForEach(resource => resource.setState(Resource.State.Idle)));
                    if (RealisticStorage.settings.hardcoreMode)
                    {
                        //generates a list of resources that are vegetables/meals/drinks, then destroys the elements of that list
                        //To-do: test this
                        storedResources.GetSlots().ForEach(slot => slot.GetResources().ToList().Where(resource => resource.getResourceType() is Vegetables || resource.getResourceType() is Meal || resource.getResourceType() is AlcoholicDrink).ToList().ForEach(resource => resource.destroy()));
                    }
                }
            }
        }
    }
}
