using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityEngine.Windows;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace ColonistEviction
{
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Eviction Key bind")] public KeyCode EvictionKeybind = KeyCode.M;
        [Draw("Eviction quick mode (delete evicted colonist without animations)")] public bool EvictionQuickMode = true;
        [Draw("Stuck rescue key bind")] public KeyCode StuckRescueKeybind = KeyCode.T;
        [Draw("Stuck rescue quick mode (teleport colonist to closest landing pad without animations)")] public bool StuckRescueQuickMode = true;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class ColonistEviction : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public new static void Init(ModEntry modEntry)
        {
            settings = ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new ColonistEviction(), modEntry, "ColonistEviction");
        }
        static void OnGUI(ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here for now
        }
        public void RegisterStrings()
        {
            //To-do: get an instance of type Colonist that won't throw a NullReferenceException
            StringUtils.RegisterString("message_eviction", GetMessageContent(null));
            StringUtils.RegisterString("message_eviction_error","Only colonists that aren't KOed can be evicted.");
        }
        public static string GetMessageContent(Colonist colonist)
        {
            return $"Evicted {colonist.getName()} from colony. Specialization of evicted colonist: {colonist.getSpecialization()}";
        }
    }
    public class CustomModule : Module
    {
        public static Module FindClosestLandingPad(Vector3 position)
        {
            float num = float.MaxValue;
            Module result = null;
            int count = mModules.Count;
            for (int i = 0; i < count; i++)
            {
                Module module = mModules[i];
                float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
                if (sqrMagnitude < num && module.getModuleType() is ModuleTypeLandingPad)
                {
                    result = findOperational(module.getPosition(), 1, 0);
                    num = sqrMagnitude;
                    if (result == null || result.getModuleType() is not ModuleTypeLandingPad)
                    {
                        result = findOperational(module.getPosition(), 16384, 0);
                    }
                }
                else if (sqrMagnitude < num && module.getModuleType() is ModuleTypeStarport)
                {
                    result = findOperational(module.getPosition(), 16384, 0);
                    num = sqrMagnitude;
                }
            }
            return result;
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.update))]
    public class ColonistEvictionPatch
    {
        public static void Postfix(Character __instance, float timeStep)
        {
            if(__instance.isSelected() && __instance.getState() != Character.State.Ko && __instance.getSpecialization() != SpecializationList.IntruderInstance && __instance.getSpecialization() != SpecializationList.VisitorInstance) //we want this to work on non-downed colonists only
            {
                if (InputAction.isValidKey(ColonistEviction.settings.EvictionKeybind))
                {
                    //the part after || is there in case colonist is stuck in a dead space between modules and corridors
                    if (ColonistEviction.settings.EvictionQuickMode || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        __instance.destroyInteractions();
                        __instance.destroy();
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction", ColonistEviction.GetMessageContent(__instance as Colonist)), ResourceList.StaticIcons.Disable, 8));
                    }
                    else if(ColonistEviction.settings.EvictionQuickMode == false || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        //To-do: implement spawning the colonist ship (with animation) and evicted colonist walking to it, despawning and ship taking off;
                        var landingPadPosition = CustomModule.FindClosestLandingPad(__instance.getPosition());
                        var colonistShipEviction = ColonistShip.create<ColonistShip>(landingPadPosition, LandingShip.Size.Regular);
                        //not sure if this Target will work as intended (pointing to colonistShipEviction)
                        Target evictionTarget = __instance.getTarget();
                        __instance.SetTarget(evictionTarget);
                        //probably need to check if evicted colonist is close enough to ship
                        if (Vector3.Distance(__instance.getPosition(), landingPadPosition.getPosition()) < 2)
                        {
                            __instance.destroy();
                            colonistShipEviction.onTakeOff();
                            Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction", ColonistEviction.GetMessageContent(__instance as Colonist)), ResourceList.StaticIcons.Disable, 8));
                        }
                    }    
                }
                else
                {
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction_error"), ResourceList.StaticIcons.Disable, 8));
                }
                if (InputAction.isValidKey(ColonistEviction.settings.StuckRescueKeybind))
                {
                    if(ColonistEviction.settings.StuckRescueQuickMode || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        __instance.destroyInteractions();
                        __instance.setPosition(CustomModule.FindClosestLandingPad(__instance.getPosition()).getPosition());
                    }
                    else if(ColonistEviction.settings.StuckRescueQuickMode == false || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        //To-do: implement a colonist ship spawning above the stuck colonist, despawn it (while keeping information about colonist like race, gender, name and class) then move the ship to landing pad, respawn the colonist and make the ship depart
                    }
                }
            }
        }
    }
}
