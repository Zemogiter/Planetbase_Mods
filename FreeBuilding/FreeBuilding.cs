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
        public static bool Prefix(Vector3 position, Vector3 normal, float size)
        {
            float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
            float heightDiff = position.y - floorHeight;

            bool isMine = MTnew.hasFlag2(ModuleType.FlagMine);
            if (isMine)
            {
                if (heightDiff < 1f || heightDiff > 3f)
                {
                    // mine must be a little elevated
                    return false;
                }
            }
            else if (heightDiff > 0.1f || heightDiff < -0.1f)
            {
                // not at floor level
                return false;
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
                    return false;
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
                        return false;
                    }
                }
            }
            // Can only be 375 units away from center of map
            Vector2 mapCenter = new Vector2(2000f, 2000f) * 0.5f;
            float distToCenter = (mapCenter - new Vector2(position.x, position.z)).magnitude;
            if (distToCenter > 375f)
            {
                return false;
            }
            //To-do: add a Reflection to Module.anyPotentialLinks because it's private
            //object[] newPosition = { position };
            //bool links = (bool)typeof(Module).GetMethod("anyPotentialLinks", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(new Module(), null);
            var module = new Module();

            if (Construction.getCount() > 1 && !CoreUtils.InvokeMethod<Module, bool>("anyPotentialLinks", module ,position))
            {
                return false;
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
                        float distToConstruction = (position - construction.getPosition()).magnitude - StaticModule.getRadius() - construction.getRadius();
                        if (distToConstruction < 3f)
                        {
                            return false;
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
                return false;
            }

            return false;
        }
    }
    /*[HarmonyPatch(typeof(Connection), nameof(Connection.canLink))]
    public class ConnectionPatch
    {
        public static bool Prefix(StaticModule m1, StaticModule m2, Vector3 position1, Vector3 position2)
        {
            if (m1.getLinkCount() == 0 && m2.getLinkCount() == 0 && Construction.mConstructions.Count > 2)
            {
                return false;
            }
            if (Construction.getInteriorConstructionCount() > 1 && m1.getLocation() != m2.getLocation())
            {
                if (m1.getLocation() == Location.Interior && m1.getLinkCount() == 0)
                {
                    return false;
                }
                if (m2.getLocation() == Location.Interior && m2.getLinkCount() == 0)
                {
                    return false;
                }
            }
            if (m1.hasFlag(1024) && m1.getInteriorLinkCount() >= 1 && !m2.getModuleType().isExterior())
            {
                return false;
            }
            if (m2.hasFlag(1024) && m2.getInteriorLinkCount() >= 1 && !m1.getModuleType().isExterior())
            {
                return false;
            }
            if (m1.isLinkedTo(m2))
            {
                return false;
            }
            if ((position1 - position2).magnitude - m1.getRadius() - m2.getRadius() > 20f)
            {
                return false;
            }
            if (!checkSweep(m1, m2, position1, position2))
            {
                return false;
            }
            if (!checkSweep(m2, m1, position1, position2))
            {
                return false;
            }

            return false;
        }
    }*/
    public class StaticModule : Module
    {
        static new int mSizeIndex;
        public override bool hasFlag(int flag)
        {
            return mModuleType.hasFlag(flag);
        }
        public static new float getRadius()
        {
            return ValidSizes[mSizeIndex] * 0.5f;
        }
    }
    public class MTnew : ModuleType
    {
        static FieldInfo mFlagsGet = typeof(ModuleType).GetField("mFlags", BindingFlags.NonPublic | BindingFlags.Static);

        static new int mFlags = Convert.ToInt32(mFlagsGet);
        public static bool hasFlag2(int flag)
        {
            return (mFlags & flag) != 0;
        }
    }
}
