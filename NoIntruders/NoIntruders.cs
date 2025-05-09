﻿using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace NoIntruders
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Enable for challenges?")] public bool affectChallenges = true;
        [Draw("Debug mode")] public bool debugMode = true;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class NoIntruders : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new NoIntruders(), modEntry, "NoIntruders");
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
            //nothing to do here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            //nothing to do here
        }
    }
	[HarmonyPatch(typeof(Planet), nameof(Planet.getIntruderMinPrestige))]
    public class PlanetPatch
    {
        public static void Prefix(Planet __instance)
        {
            if (ChallengeManager.getInstance().isChallengeEnabled() && NoIntruders.settings.affectChallenges == false)
            {
                if (NoIntruders.settings.debugMode)
                {
                    Console.WriteLine("NoIntruders - Challenge active and the mod is currently set to not affect them.");
                    Console.WriteLine("NoIntruders - value of mIntruderMinPrestige" + CoreUtils.GetMember<Planet, float>("mIntruderMinPrestige", __instance).ToString());
                }
                return;
            }
            CoreUtils.SetMember("mIntruderMinPrestige", __instance, 2000f);
            if (NoIntruders.settings.debugMode) Console.WriteLine("NoIntruders - minimum intruder prestige after changes: " + CoreUtils.GetMember<Planet, float>("mIntruderMinPrestige", __instance));
        }
    }
}
