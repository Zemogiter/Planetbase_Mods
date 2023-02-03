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
            Console.WriteLine("MeteorPatch.Prefix position is = " + terrainPosition);
            Singleton<MeteorManager>.getInstance().remove(__instance.gameObject);
            
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == 10; i++)
            {
                Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(MeteorBehaviour), nameof(MeteorBehaviour.Update))]
    public class MetorPatch2
    {
        static bool Prefix(MeteorBehaviour __instance)
        {
            if (__instance.mCollided)
            {
                __instance.mDestroyTimer -= Time.deltaTime;
                if (__instance.mDestroyTimer <= 0f)
                {
                    PhysicsUtil.findFloor(__instance.gameObject.transform.position, out var terrainPosition, out var normal);
                    Console.WriteLine("MeteorPatch2.Prefix position is = " + terrainPosition);
                    ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
                    for (int i = 0; i == 10; i++)
                    {
                        Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
                    }
                    __instance.remove();
                }
            }
            if (__instance.gameObject.transform.position.y < -100f)
            {
                __instance.remove();
            }
            if (__instance.transform.position.y < 0f)
            {
                __instance.doCollision();
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(MeteorBehaviour), nameof(MeteorBehaviour.doCollision))]
    public class MeteorPatch3
    {
        static bool Prefix(MeteorBehaviour __instance) 
        {
            if (__instance.mCollided)
            {
                return false;
            }
            __instance.mCollided = true;
            if (PhysicsUtil.findFloor(__instance.gameObject.transform.position, out var terrainPosition, out var normal))
            {
                bool flag = false;
                int layerMask = 4198400;
                Ray ray = new Ray(terrainPosition + Vector3.up * 20f, Vector3.down);
                RaycastHit[] array = Physics.SphereCastAll(ray, 2f, 40f, layerMask);
                RaycastHit hitInfo;
                bool flag2 = Physics.SphereCast(ray, 8f, out hitInfo, 40f, layerMask);
                Construction construction = null;
                float num = float.MaxValue;
                foreach (RaycastHit raycastHit in array)
                {
                    GameObject gameObject = raycastHit.collider.transform.root.gameObject;
                    Construction construction2 = Construction.find(gameObject);
                    if (construction2 != null)
                    {
                        float num2 = (construction2.getPosition() - terrainPosition).magnitude - construction2.getLongRadius();
                        if (num2 < 2f && num2 < num)
                        {
                            construction = construction2;
                            num = num2;
                        }
                    }
                }
                if (construction != null)
                {
                    construction.onImpact(ImpactType.Meteor);
                    flag = true;
                    __instance.gameObject.GetComponent<Renderer>().enabled = false;
                }
                Character.onImpact(ImpactType.Meteor, terrainPosition);
                __instance.transform.position = terrainPosition;
                Object.Destroy(__instance.GetComponent<Rigidbody>());
                __instance.transform.GetChild(0).gameObject.SetActive(value: true);
                ParticleSystem[] componentsInChildren = __instance.transform.GetChild(1).gameObject.GetComponentsInChildren<ParticleSystem>();
                for (int j = 0; j < componentsInChildren.Length; j++)
                {
                    componentsInChildren[j].enableEmission = false;
                }
                if (!flag && !flag2 && terrainPosition.y < 0.5f)
                {
                    GameObject meteorCrater = PlanetManager.getCurrentPlanet().getDefinition().MeteorCrater;
                    if (meteorCrater != null)
                    {
                        GameObject gameObject2 = Object.Instantiate(meteorCrater);
                        gameObject2.transform.position = terrainPosition;
                        gameObject2.transform.up = normal;
                        gameObject2.transform.SetParent(__instance.transform, worldPositionStays: true);
                    }
                }
            }
            __instance.mAudioSource.Stop();
            MeteorSoundDefinition meteorHit = SoundList.getInstance().MeteorHit;
            __instance.applyFilterParameters(meteorHit);
            __instance.mAudioSource.play(meteorHit, loop: false, Singleton<Profile>.getInstance().getSfxVolumeNormalized());

            Console.WriteLine("MeteorPatch3.Prefix position is = " + terrainPosition);
            ResourceType oreType = TypeList<ResourceType, ResourceTypeList>.find<Ore>();
            for (int i = 0; i == 10; i++)
            {
                Resource meteorOre = Resource.create(oreType, terrainPosition, Location.Exterior);
            }

            return false;
        }

    }
}
