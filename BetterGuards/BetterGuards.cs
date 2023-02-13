﻿using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Data;
using UnityModManagerNet;
using System.Reflection;

namespace BetterGuards
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Health multipler for guards")] public float healthmult = 4.2f;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class BetterGuards : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new BetterGuards(), modEntry, "BetterGuards");
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
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    [HarmonyPatch(typeof(Character))]
    [HarmonyPatch("mIndicators", MethodType.Setter)]
    public class GuardPatch
    {
        //replacing the default health bar with a larger one for guards
        public static void Postfix(Character __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>())
            {
                Indicator indicator = new(StringList.get("health"), ResourceList.StaticIcons.Health, IndicatorType.Normal, 1f, BetterGuards.settings.healthmult, SignType.Health);
                indicator.setLevels(0.1f, 0.5f, 0.7f, 0.8f);
                indicator.setOrientation(IndicatorOrientation.Vertical);
                __instance.mIndicators[0] = indicator;
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.onKo))]
    public class CharacterPatch
    {
        //make guards and intruders drop guns on death (vanilla = guns disapear with guards/intruders)
        public static void Postfix(Character __instance)
        {
            if(__instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>() || __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
            {
                Vector3 position = __instance.getPosition();
                Location location = __instance.getLocation();
                ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();
                Resource droppedGun = Resource.create(gunType, position, location);
            }
        }
    }
}
