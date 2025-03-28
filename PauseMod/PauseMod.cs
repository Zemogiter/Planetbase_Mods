using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using System;
using System.Reflection;

namespace PauseMod
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Pause the game on window focus loss?")] public bool pauseOnFocusLoss = true;
        [Draw("Pause keybind")] public KeyCode pauseKeybind = KeyCode.T;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class PauseMod : ModBase
    {
        public static bool ShouldOverrideTimeManagerIsPaused { get; set; } = false;
        public static bool enabled;
        public static Settings settings;
        public static bool ActiveDeliveryShip { get; set; }
        public static ColonyShip Ship { get; set; }

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new PauseMod(), modEntry, "PauseMod");
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
            //nothing to do here
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("menu_pause", "Pause game");
        }
        public bool FocusLossHandler()
        {
            if (settings.pauseOnFocusLoss && !Application.isFocused)
            {
                return true;
            }
            return false;
        }
    }
}
