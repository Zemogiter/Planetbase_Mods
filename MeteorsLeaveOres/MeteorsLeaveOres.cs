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
            if(GameManager.getInstance().getGameState() is GameStateGame && MeteorManager.getInstance().spawnMeteor != null)
            {
                PhysicsUtil.findFloor(MeteorManager.getInstance().mMeteors.First().gameObject.transform.position, out var terrainPosition, out var normal);
                CustomMeteor.OreSpawn(terrainPosition);
            }
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
                Resource meteorOre = Resource.create(oreType, position, Location.Exterior);
            }
        }
    }
    [HarmonyPatch(typeof(MeteorManager), nameof(MeteorManager.remove))]
    public class MeteorPatch4
    {
        private static List<GameObject> mMeteors = new List<GameObject>();
        public static bool Prefix(GameObject meteor)
        {
            PhysicsUtil.findFloor(meteor.transform.position, out var terrainPosition, out var normal);
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == MeteorsLeaveOres.settings.oreCount; i++)
            {
                Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
            }
            mMeteors.Remove(meteor);
            UnityEngine.Object.Destroy(meteor);
            return false;
        }
    }
    [HarmonyPatch(typeof(MeteorManager), nameof(MeteorManager.spawnMeteor))]
    public class MeteorPatch5 : MeteorManager
    {
        public static bool Prefix([Optional] Vector3 target)
        {
            System.Random random = new System.Random(mSeeds[0]);
            mIndex++;
            GameObject[] prefabMeteors = ResourceList.getInstance().PrefabMeteors;
            GameObject gameObject = UnityEngine.Object.Instantiate(prefabMeteors[random.Next(0, prefabMeteors.Length)]);
            Vector3 center = Singleton<TerrainGenerator>.getInstance().getCenter();
            gameObject.setLayerRecursive(9);
            float num = random.range(0.75f, 1.25f);
            gameObject.transform.localScale = new Vector3(num, num, num);
            gameObject.name = "Meteor " + mIndex;
            Rigidbody component = gameObject.GetComponent<Rigidbody>();
            if (target == default(Vector3))
            {
                gameObject.transform.position = center + new Vector3(random.range(-700f, 700f), 200f, random.range(-700f, 700f));
                component.velocity = new Vector3(random.range(-100f, 100f), 0f - random.range(80f, 100f), random.range(-100f, 100f));
            }
            else
            {
                gameObject.transform.position = target + new Vector3(random.range(-200f, 200f), 200f, random.range(-200f, 200f));
                component.velocity = (target - component.position).normalized * random.range(80f, 100f);
            }
            component.angularVelocity = new Vector3(random.range(-10f, 10f), random.range(-10f, 10f), random.range(-10f, 10f));
            gameObject.transform.localRotation = Quaternion.LookRotation(-component.velocity.normalized);
            if (random.range(0f, 1f) <= Singleton<Colony>.getInstance().getDisasterInterceptionChance() || Singleton<DebugManager>.getInstance().is100PercentInterceptionChance())
            {
                Vector3 position = gameObject.transform.position + component.velocity * 2f;
                Module.findLaser(position)?.setTargetMeteor(gameObject);
            }
            mTime = 0f;
            mSeeds.RemoveAt(0);
            generateSeeds();
            mMeteors.Add(gameObject);
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == MeteorsLeaveOres.settings.oreCount; i++)
            {
                Resource meteorOre = Resource.create(oreType, position, Location.Exterior);
            }
            return false;
        }
    }
}
