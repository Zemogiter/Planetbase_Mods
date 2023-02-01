using Planetbase;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using Module = Planetbase.Module;
using System;

namespace FreeBuilding
{
    public class FreeBuilding : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new FreeBuilding(), modEntry, "FreeBuilding");

        public override void OnInitialized(ModEntry modEntry)
        {
            
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
        }
    }
    [HarmonyPatch(typeof(Module), nameof(Module.canPlaceModule))]
    public class ModulePatch
    {
        public static void Postfix(Vector3 position, Vector3 normal, float size, Module __instance)
        {
            float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
            float heightDiff = position.y - floorHeight;

            bool isMine = __instance.hasFlag(ModuleType.FlagMine);
            if (isMine)
            {
                if (heightDiff < 1f || heightDiff > 3f)
                {
                    // mine must be a little elevated
                    return;
                }
            }
            else if (heightDiff > 0.1f || heightDiff < -0.1f)
            {
                // not at floor level
                return;
            }
            // here we're approximating the circumference of the structure with 8 points and will check that all these points are level with the floor
            float reducedRadius = size * 0.75f;
            float angledReducedRadius = reducedRadius * 1.41421354f * 0.5f;
            Vector3[] circumference = new Vector3[]
            {
                position + new Vector3(reducedRadius, 0f, 0f),
                position + new Vector3(-reducedRadius, 0f, 0f),
                position + new Vector3(0f, 0f, reducedRadius),
                position + new Vector3(0f, 0f, -reducedRadius),
                position + new Vector3(angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(angledReducedRadius, 0f, -angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, -angledReducedRadius)
            };
            if (isMine)
            {
                // above we verified that it is a bit elevated
                // now make sure that at least one point is near level ground
                bool isValid = false;
                for (int i = 0; i < circumference.Length; i++)
                {
                    PhysicsUtil.findFloor(circumference[i], out Vector3 floor, 256);
                    if (floor.y < floorHeight + 1f || floor.y > floorHeight - 1f)
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    return;
                }
            }
            else
            {
                // Make sure all points are near level ground
                for (int j = 0; j < circumference.Length; j++)
                {
                    PhysicsUtil.findFloor(circumference[j], out Vector3 floor, 256);
                    if (floor.y > floorHeight + 2f || floor.y < floorHeight - 1f)
                    {
                        return;
                    }
                }
            }
            // Can only be 375 units away from center of map
            Vector2 mapCenter = new Vector2(2000f, 2000f) * 0.5f;
            float distToCenter = (mapCenter - new Vector2(position.x, position.z)).magnitude;
            if (distToCenter > 375f)
            {
                return;
            }

            if (Construction.getCount() > 1 && !CoreUtils.InvokeMethod<Module, bool>("anyPotentialLinks", __instance ,position))
            {
                return;
            }

            RaycastHit[] array2 = Physics.SphereCastAll(position + Vector3.up * 20f, size * 0.5f + 3f, Vector3.down, 40f, 4198400);
            if (array2 != null)
            {
                for (int k = 0; k < array2.Length; k++)
                {
                    RaycastHit raycastHit = array2[k];
                    GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    var dictionary = CoreUtils.GetMember<Construction, Dictionary<GameObject, Construction>>("mConstructionDictionary");
                    Construction construction = dictionary[gameObject];
                    if (construction != null)
                    {
                        float distToConstruction = (position - construction.getPosition()).magnitude - __instance.getRadius() - construction.getRadius();
                        if (distToConstruction < 3f)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Not hitting construction: " + gameObject.name);
                    }
                }
            }

            // Check that it's away from the ship
            if (Physics.CheckSphere(position, size * 0.5f + 3f, 65536))
            {
                return;
            }

            return;
        }
    }
}
