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

namespace MeteorsLeaveOres
{
    public class MeteorsLeaveOres : ModBase
    {
        public static int oreCount;
        public static new void Init(ModEntry modEntry) => InitializeMod(new MeteorsLeaveOres(), modEntry, "MeteorsLeaveOres");

        public override void OnInitialized(ModEntry modEntry)
        {
            var path = "./Mods/MeteorsLeaveOres/config.txt";
            string line;
            System.IO.StreamReader file = new(path);
            line = file.ReadLine();
            line = line.Substring(10);
            oreCount = int.Parse(line);
            Console.WriteLine("The value of oreCount is " + oreCount + " of type " + oreCount.GetType());
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            /*if(GameManager.getInstance().getGameState() is GameStateGame && MeteorManager.getInstance().mMeteors != null)
            {
                var meteor = new CustomMeteor();
                meteor.OreSpawn();
            }*/
        }
    }
    public class CustomMeteor : MeteorBehaviour
    {
        public void OreSpawn()
        {
            PhysicsUtil.findFloor(gameObject.transform.position, out var terrainPosition, out var normal);
            Console.WriteLine("CustomMeteor.OreSpawn position is = " + terrainPosition);
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == MeteorsLeaveOres.oreCount; i++)
            {
                Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
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
            for (int i = 0; i == MeteorsLeaveOres.oreCount; i++)
            {
                Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
            }
            mMeteors.Remove(meteor);
            UnityEngine.Object.Destroy(meteor);
            return false;
        }
    }
    [HarmonyPatch(typeof(MeteorManager), nameof(MeteorManager.spawnMeteor))]
    public class MeteorPatch5
    {
        public static void Postfix([Optional] Vector3 target)
        {
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == MeteorsLeaveOres.oreCount; i++)
            {
                Resource meteorOre = Resource.create(oreType, target, Location.Exterior);
            }
        }
    }
}
