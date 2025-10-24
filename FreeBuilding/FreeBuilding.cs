using System;
using System.Collections.Generic;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static Planetbase.GameManager;
using static UnityModManagerNet.UnityModManager;
using Module = Planetbase.Module;

namespace FreeBuilding
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Construction rotation keybind")] public KeyCode ConstructionRotation = KeyCode.T;
        [Draw("Disable modules upon construction?")] public bool TurnOffOnBuilt = false;
        [Draw("Experimental Mode (lifts airlock placement restrictions, very buggy)")] public bool ExperimentalMode = false;
        [Draw("Debug Mode (prints debug messages)")] public bool DebugMode = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class FreeBuilding : ModBase
    {
        private int connectionCount;
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new FreeBuilding(), modEntry, "FreeBuilding");
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
            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().hasFlag(ModuleType.FlagAutoRotate);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //allows module rotation pre-placement
            var managerInstance = GameManager.getInstance();
            if (managerInstance != null && managerInstance.getGameState() is GameStateGame gameState)
            {
                var state = CoreUtils.GetMember<GameManager, State>("mState", managerInstance);
                if (state != GameManager.State.Updating)
                    return;

                Module activeModule = CoreUtils.GetMember<GameStateGame, Module>("mActiveModule", gameState);
                if (activeModule == null)
                    return;
                List<Vector3> connectionPositions = new List<Vector3>();
                var constructionList = BuildableUtils.GetAllModules();
                for (int i = 0; i < constructionList.Count; i++)
                {
                    if (constructionList[i] != null && activeModule != null && constructionList[i] != activeModule && Connection.canLink(activeModule, constructionList[i], activeModule.getPosition(), constructionList[i].getPosition()))
                    {
                        connectionPositions.Add(constructionList[i].getPosition());
                    }
                }
                if (connectionPositions.Count == 0)
                    return;

                connectionCount = Math.Min(connectionCount, connectionPositions.Count - 1);
                if (Input.GetKeyDown(settings.ConstructionRotation))
                {
                    connectionCount = ++connectionCount % connectionPositions.Count;
                }

                activeModule.getGameObject().transform.localRotation = Quaternion.LookRotation((connectionPositions[connectionCount] - activeModule.getPosition()).normalized);
            }
            else
            {
                return;
            }
        }
    }
    [HarmonyPatch(typeof(Module), nameof(Module.onBuilt))]
    public class OnBuiltPatch
    {
        public static void Postfix(Module __instance)
        {
            //turns building off upon construction
            if (FreeBuilding.settings.TurnOffOnBuilt == true)
            {
                __instance.setEnabled(false);
            }
            //instantly builds connection to a module once it's done (still requires resources to be placed, it's a QoL feature to prevent builders from getting stuck)
            foreach (Module module in BuildableUtils.GetAllModules())
            {
                foreach (Construction connection in module.getLinks())
                {
                    if (connection.isAwaitingBuilder())
                    {
                        connection.onBuilt();
                    }
                }
            }
        }
    }
    //main code
    [HarmonyPatch(typeof(Module), nameof(Module.canPlaceModule))]
    public class ModulePatch
    {
        public static bool Prefix(Module __instance, ref bool __result, Vector3 position, Vector3 normal, float size)
        {
            __result = ReplacementMethod(__instance, position, normal, size);
            return false;
        }
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool ReplacementMethod(Module __instance,Vector3 position, Vector3 normal, float size)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
            float heightDiff = position.y - floorHeight;

            bool isMine = __instance.hasFlag(ModuleType.FlagMine);
            bool isAirlock = __instance.hasFlag(ModuleType.FlagAirlock);

            if (isAirlock && GameManager.getInstance().getGameState() is GameStateGame && FreeBuilding.settings.ExperimentalMode )
            {
                return true;
            }

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
            Vector3[] circumference =
            [
                position + new Vector3(reducedRadius, 0f, 0f),
                position + new Vector3(-reducedRadius, 0f, 0f),
                position + new Vector3(0f, 0f, reducedRadius),
                position + new Vector3(0f, 0f, -reducedRadius),
                position + new Vector3(angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(angledReducedRadius, 0f, -angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, -angledReducedRadius)
            ];
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

            if (Construction.getCount() > 1 && !CoreUtils.InvokeMethod<Module, bool>("anyPotentialLinks", __instance ,position))
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
                        float distToConstruction = (position - construction.getPosition()).magnitude - __instance.getRadius() - construction.getRadius();
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

            return true;
        }
    }
}
