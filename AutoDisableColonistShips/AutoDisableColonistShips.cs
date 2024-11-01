﻿using System;
using System.Collections.Generic;
using System.Timers;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace AutoDisableColonistShips
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Trigger Value (max oxygen genration - oxygen usage)")] public int TriggerValue = 4;
        [Draw("Re-enable ships once you go above trigger value?")] public bool ReEnableShips = false;
        [Draw("Disallow visitor ships as well?")] public bool DisallowVisitorShips = false;
        [Draw("Manual Override (enable colonist/visitor ships even when below trigger value)")] public bool manualOverride = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class AutoDisableColonistShips : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        //needed so that we wont display the same message over and over
        public static bool messageDisplayed = false;
        public static bool messageDisplayedNormal = false;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new AutoDisableColonistShips(), modEntry, "AutoDisableColonistShips");
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
        public const string MESSAGE = "Geting low on oxygen, disallowing colonist ships from landing.";
        public const string MESSAGE2 = "Geting low on oxygen, disallowing colonist and visitor ship from landing.";
        public const string MESSAGE3 = "Oxygen levels green, colonist ships are allowed entry.";
        public const string MESSAGE4 = "Oxygen levels green, colonist and visitor ships are allowed entry.";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if(GameManager.getInstance().getGameState() is GameStateGame)
            {
                var landingPermissions = LandingShipManager.getInstance().getLandingPermissions();
                var refBool = landingPermissions.getColonistRefBool();
                var refBoolVisitors = landingPermissions.getVisitorRefBool();

                if(settings.manualOverride == true)
                {
                    return;
                }
                //we also need to check if we even have oxygen generators on map and basic base functions covered to avoid unnecessary messages
                if (Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeOxygenGenerator>()) > 0 && Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeWaterExtractor>()) > 0 && Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeSolarPanel>()) > 0 || Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeWindTurbine>()) > 0)
                {
                    //without visitor ships
                    if (refBool.mValue == true && settings.DisallowVisitorShips == false && CountOxygenUsers() <= settings.TriggerValue)
                    {
                        refBool.set(false);
                        if (messageDisplayed == false)
                        {
                            Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled",MESSAGE), ResourceList.StaticIcons.Oxygen, 1));
                            messageDisplayed = true;
                        }
                        if (settings.ReEnableShips == true && settings.DisallowVisitorShips == false && CountOxygenUsers() >= settings.TriggerValue && messageDisplayedNormal == false)
                        {
                            refBool.set(true);
                            Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal", MESSAGE3), ResourceList.StaticIcons.Oxygen, 8));
                            messageDisplayedNormal = true;
                        }
                    }    
                    //with vistior ships
                    else if (refBool.mValue == true && settings.DisallowVisitorShips == true && CountOxygenUsers() <= settings.TriggerValue)
                    {
                        refBool.set(false);
                        refBoolVisitors.set(false);
                        if (messageDisplayed == false)
                        {
                            Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled_2",MESSAGE2), ResourceList.StaticIcons.Oxygen, 8));
                            messageDisplayed = true;
                        }
                        if (settings.ReEnableShips == true && settings.DisallowVisitorShips == true && CountOxygenUsers() >= settings.TriggerValue && messageDisplayedNormal == false)
                        {
                            refBool.set(true);
                            refBoolVisitors.set(true);
                            Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal_2", MESSAGE4), ResourceList.StaticIcons.Oxygen, 8));
                            messageDisplayedNormal = true;
                        }
                    }
                    messageDisplayedNormal= false;
                    messageDisplayed = false;
                }
            }
        }

        private static void RegisterStrings()
        {
            StringUtils.RegisterString("message_low_oxygen_landing_disabled", MESSAGE);
            StringUtils.RegisterString("message_low_oxygen_landing_disabled_2", MESSAGE2);
            StringUtils.RegisterString("message_oxygen_level_normal", MESSAGE3);
            StringUtils.RegisterString("message_oxygen_level_normal_2", MESSAGE4);
        }
        public int CountOxygenUsers()
        {
            int numberofColonists = Character.getHumanCount();
            int maxNumber = Module.getOverallOxygenGeneration();

            return maxNumber - numberofColonists;
        }
    }
}
