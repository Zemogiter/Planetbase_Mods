using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Mono.Cecil;
using ResourceType = Planetbase.ResourceType;
using Resource = Planetbase.Resource;
using System.Reflection;
using Object = UnityEngine.Object;

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
            //line = line.Substring(12);
            line = line.Substring(line.IndexOf("=") + 1);
            oreCount = int.Parse(line);
            Console.WriteLine("The value of oreCount is " + oreCount + " of type " + oreCount.GetType());
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(MeteorBehaviour), nameof(MeteorBehaviour.remove))]
    public class MeteorPatch
    {
        
        static bool Prefix(MeteorBehaviour __instance)
        {
            PhysicsUtil.findFloor(__instance.gameObject.transform.position, out var terrainPosition, out var normal);
            Singleton<MeteorManager>.getInstance().remove(__instance.gameObject);
            
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == 10; i++)
            {
                Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
            }
            return false;
        }
    }
}
