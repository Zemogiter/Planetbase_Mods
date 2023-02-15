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
        [Draw("Eviction Keybind")] public KeyCode evictionKeybind = KeyCode.F;
        [Draw("Quick mode (delete colonist without animations)")] public bool quickMode = true;
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

        public const string MESSAGE = "Colonist evicted. ";

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
            StringUtils.RegisterString("message_eviction", MESSAGE);
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
            if(__instance.mSelected == true && __instance.getState() != Character.State.Ko && __instance.mSpecialization != SpecializationList.IntruderInstance && __instance.mSpecialization != SpecializationList.VisitorInstance)
            {
                if (Input.GetKeyUp(ColonistEviction.settings.evictionKeybind))
                {
                    //the part after || is there in case colonist is stuck in a dead space between modules and corridors
                    if (ColonistEviction.settings.quickMode == true || __instance.getState() == Character.State.Idle && __instance.getLocation() == Location.Exterior)
                    {
                        __instance.destroy();
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction" + __instance.getName() + __instance.getSpecialization(), ColonistEviction.MESSAGE), ResourceList.StaticIcons.Disable, 8));
                    }
                    else 
                    {
                        //To-do: implement spawning the colonist ship (with animation) and evicted colonist walking to it, despawning and ship taking off;
                        var landingPadPosition = CustomModule.findClosestLandingPad(__instance.getPosition());
                        var colonistShipEviction = ColonistShip.create<ColonistShip>(landingPadPosition, LandingShip.Size.Regular);
                        __instance.setTarget(landingPadPosition);
                        //probably need to check if evicted colonist is close enough to ship
                        if (Vector3.Distance(__instance.getPosition(), landingPadPosition.getPosition()) < 5)
                        {
                            __instance.destroy();
                            colonistShipEviction.onTakeOff();
                            Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction" + __instance.getName() + __instance.getSpecialization(), ColonistEviction.MESSAGE), ResourceList.StaticIcons.Disable, 8));
                        }
                    }    
                }
            }
        }
    }
}
