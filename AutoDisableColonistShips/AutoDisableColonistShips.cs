using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace AutoDisableColonistShips
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Trigger Value")] public int TriggerValue = 4;
        [Draw("Re-enable Ships")] public bool ReEnableShips = false;
        [Draw("Disallow Visitor Ships")] public bool DisallowVisitorShips = false;
        [Draw("Time between oxygen notifications(miliseconds)")] public int TimeBetweenOxygenNotifications = 20000;
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
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle= OnToggle;

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
            //nothing required here
        }

        private static void RegisterStrings()
        {
            StringUtils.RegisterString("message_low_oxygen_landing_disabled", MESSAGE);
            StringUtils.RegisterString("message_low_oxygen_landing_disabled_2", MESSAGE2);
            StringUtils.RegisterString("message_oxygen_level_normal", MESSAGE3);
            StringUtils.RegisterString("message_oxygen_level_normal_2", MESSAGE4);
        }
        public static void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            // Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                // Console.WriteLine("stop wait timer");
            };

            while (timer1.Enabled)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
    [HarmonyPatch(typeof(LandingPermissions), nameof(LandingPermissions.areColonistsAllowed))]
    public class LandingPermissionsPatch : LandingPermissions
    {
        //code to disable arrival of colonist and visitor ships if we have less than 4 left in oxygen production
        //to-do: make the display of MESSAGE/MESSAGE2 less frequent (low priotiry)
        public static void Postfix(LandingPermissions __instance)
        {
            var refBool = __instance.getColonistRefBool();
            var refBoolVisitors = __instance.getVisitorRefBool();

            if (GameManager.getInstance().getGameState() is GameStateGame)
            {
                int numberofColonists = Character.getHumanCount();
                int maxNumber = Module.getOverallOxygenGeneration();

                if (AutoDisableColonistShips.settings.DisallowVisitorShips == false && maxNumber - numberofColonists < AutoDisableColonistShips.settings.TriggerValue)
                {
                    Console.WriteLine("We reached the trigger value and disallowing visitor ship is off");
                    refBool.set(false);
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled", AutoDisableColonistShips.MESSAGE), ResourceList.StaticIcons.Oxygen, 8));
                    while (maxNumber - numberofColonists < AutoDisableColonistShips.settings.TriggerValue)
                    {
                        AutoDisableColonistShips.wait(20000);
                    }
                    if (AutoDisableColonistShips.settings.ReEnableShips == true &&  maxNumber - numberofColonists > AutoDisableColonistShips.settings.TriggerValue)
                    {
                        refBool.set(true);
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal", AutoDisableColonistShips.MESSAGE3), ResourceList.StaticIcons.Oxygen, 1));
                    }
                }
                else if (AutoDisableColonistShips.settings.DisallowVisitorShips == true && maxNumber - numberofColonists < AutoDisableColonistShips.settings.TriggerValue)
                {
                    Console.WriteLine("We reached the trigger value and disallowing visitor ship is on");
                    refBool.set(false);
                    refBoolVisitors.set(false);
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled_2", AutoDisableColonistShips.MESSAGE2), ResourceList.StaticIcons.Oxygen, 8));
                    while (maxNumber - numberofColonists < AutoDisableColonistShips.settings.TriggerValue)
                    {
                        AutoDisableColonistShips.wait(20000);
                    }
                    if (AutoDisableColonistShips.settings.ReEnableShips == true && maxNumber - numberofColonists > AutoDisableColonistShips.settings.TriggerValue)
                    {
                        refBool.set(true);
                        refBoolVisitors.set(true);
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal_2", AutoDisableColonistShips.MESSAGE4), ResourceList.StaticIcons.Oxygen, 1));
                    }
                }
            }
        }
    }
}
