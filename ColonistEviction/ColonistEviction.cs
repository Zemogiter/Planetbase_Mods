using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine.UI;
using Selectable = Planetbase.Selectable;

namespace ColonistEviction
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Eviction Keybind")] public KeyCode evictionKeybind = KeyCode.M;
        [Draw("Eviction quick mode (delete evicted colonist without animations)")] public bool EvictionQuickMode = true;
        [Draw("Stuck rescue keybind")] public KeyCode stuckRescueKeybind = KeyCode.T;
        [Draw("Stuck rescue quick mode (teleport colonist to closest landing pad without animations)")] public bool stuckRescueQuickMode = true;
        public override void Save(UnityModManager.ModEntry modEntry)
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
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new ColonistEviction(), modEntry, "ColonistEviction");
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
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here for now
        }
        public void RegisterStrings()
        {
            //To-do: get an instance of type Colonist that wont crash the game with an NRE
            Colonist colonist= (Colonist)Colonist.getFirstHuman();
            StringUtils.RegisterString("message_eviction", GetMessageContent(colonist));
            StringUtils.RegisterString("message_eviction_error","Cannot evict colonist. Can't be knocked out, an intruder or a visitor.");
        }
        public static string GetMessageContent(Colonist __instance)
        {
            return $"Evicted {__instance.getName()} from colony. Specialization of evicted colonist: {__instance.getSpecialization()}";
        }
    }
    public class CustomModule : Module
    {
        public static Module findClosestLandingPad(Vector3 position)
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
                if (Input.GetKeyUp(ColonistEviction.settings.evictionKeybind))
                {
                    //the part after || is there in case colonist is stuck in a dead space between modules and corridors
                    if (ColonistEviction.settings.EvictionQuickMode == true || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        __instance.destroyInteractions();
                        __instance.destroy();
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction", ColonistEviction.GetMessageContent(__instance as Colonist)), ResourceList.StaticIcons.Disable, 8));
                    }
                    else if(ColonistEviction.settings.EvictionQuickMode == false || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        //To-do: implement spawning the colonist ship (with animation) and evicted colonist walking to it, despawning and ship taking off;
                        var landingPadPosition = CustomModule.findClosestLandingPad(__instance.getPosition());
                        var colonistShipEviction = ColonistShip.create<ColonistShip>(landingPadPosition, LandingShip.Size.Regular);
                        //not sure if this Target will work as intended (pointing to colonistShipEviction)
                        Target evictionTarget = __instance.getTarget();
                        CharacterUtils.SetTarget(__instance,evictionTarget);
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
                if (Input.GetKeyUp(ColonistEviction.settings.stuckRescueKeybind))
                {
                    if(ColonistEviction.settings.stuckRescueQuickMode == true || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        __instance.destroyInteractions();
                        __instance.setPosition(CustomModule.findClosestLandingPad(__instance.getPosition()).getPosition());
                    }
                    else if(ColonistEviction.settings.stuckRescueQuickMode == false || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        //To-do: implement a colonist ship spawning above the stuck colonist, despawn it (while keeping information about colonist like indicators and class) then move the ship to landing pad, respawn the colonist and make the ship depart
                    }
                }
            }
        }
    }
}
