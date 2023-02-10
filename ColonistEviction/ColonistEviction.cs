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

        }
        public void RegisterStrings()
        {
            StringUtils.RegisterString("message_eviction", MESSAGE);
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
                    if (ColonistEviction.settings.quickMode == true)
                    {
                        __instance.destroy();
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_eviction" + __instance.getName() + __instance.getSpecialization(), ColonistEviction.MESSAGE), ResourceList.StaticIcons.Disable, 8));
                    }
                    else 
                    {
                        //To-do: implement spawning the colonist ship (with animation) and evicted colonist walking to it, despawning and ship taking off
                    
                    }    
                }
            }
        }
    }
}
