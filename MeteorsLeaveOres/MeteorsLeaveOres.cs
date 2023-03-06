using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using ResourceType = Planetbase.ResourceType;
using Resource = Planetbase.Resource;
using System.Reflection;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using UnityModManagerNet;
using Module = Planetbase.Module;
using System.Security.AccessControl;

namespace MeteorsLeaveOres
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Ores to spawn per meteor")] public int oreCount = 4;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class MeteorsLeaveOres : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new MeteorsLeaveOres(), modEntry, "MeteorsLeaveOres");
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
            //nothing required here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            /*if(GameManager.getInstance().getGameState() is GameStateGame)
            {
                PhysicsUtil.findFloor(MeteorManager.getInstance().mMeteors.First().gameObject.transform.position, out var terrainPosition, out var normal);
                CustomMeteor.OreSpawn(terrainPosition);
            }*/
        }
    }
    public class CustomMeteor : MeteorBehaviour
    {
        public static void OreSpawn(Vector3 position)
        {
            Console.WriteLine("CustomMeteor.OreSpawn position is = " + position);
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == MeteorsLeaveOres.settings.oreCount; i++)
            {
                Resource.create(oreType, position, Location.Exterior);
            }
        }
    }
    [HarmonyPatch(typeof(Module), nameof(Module.onImpact))]
    public class ModulePatch
    {
        static void Postfix(Module __instance, ImpactType impactType)
        {
            if(impactType is ImpactType.Meteor)
            {
                for (int i = 0; i == MeteorsLeaveOres.settings.oreCount; i++)
                {
                    ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
                    Resource.create(oreType, __instance.getPosition(), Location.Exterior);
                }
            }
        }
    }
    [HarmonyPatch(typeof(MeteorManager), nameof(MeteorManager.spawnMeteor))]
    public class MeteorManagerPatch
    {
        static void Postfix(Vector3 target)
        {
            for (int i = 0; i == MeteorsLeaveOres.settings.oreCount; i++)
            {
                ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
                Resource.create(oreType, target, Location.Exterior);
            }
        }
    }
}
