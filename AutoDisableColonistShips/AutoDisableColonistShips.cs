using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Runtime.Serialization;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AutoDisableColonistShips
{
    public class Item
    {
        public bool ReEnableShips { get; set; }
        public bool DisallowVisitorShips { get; set; }
        public int TriggerValue { get; set; }
    }
    public class AutoDisableColonistShips : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new AutoDisableColonistShips(), modEntry, "AutoDisableColonistShips");

        public const string MESSAGE = "Geting low on oxygen, disallowing colonist ships from landing.";
        public const string MESSAGE2 = "Geting low on oxygen, disallowing colonist and visitor ship from landing.";
        public const string MESSAGE3 = "Oxygen levels green, colonist ships are allowed entry.";
        public const string MESSAGE4 = "Oxygen levels green, colonist and visitor ships are allowed entry.";

        public const string path = "./Mods/AutoDisableColonistShips/config.json";

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                items.ForEach(i => Console.Write("{0}\t", i));
            }
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            LoadJson();
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
            var settings = new Item();

            if (GameManager.getInstance().getGameState() is GameStateGame game)
            {
                int numberofColonists = Character.getHumanCount();
                int maxNumber = Planetbase.Module.getOverallOxygenGeneration();

                if (maxNumber - numberofColonists < settings.TriggerValue)
                {
                    refBool.set(false);
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled", AutoDisableColonistShips.MESSAGE), ResourceList.StaticIcons.Oxygen, 4));
                    if (settings.ReEnableShips == true &&  maxNumber - numberofColonists > settings.TriggerValue)
                    {
                        refBool.set(true);
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal", AutoDisableColonistShips.MESSAGE3), ResourceList.StaticIcons.Oxygen, 4));
                    }
                }
                else if (settings.DisallowVisitorShips == true && maxNumber - numberofColonists < settings.TriggerValue)
                {
                    refBool.set(false);
                    refBoolVisitors.set(false);
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled_2", AutoDisableColonistShips.MESSAGE2), ResourceList.StaticIcons.Oxygen, 4));
                    if (settings.ReEnableShips == true && maxNumber - numberofColonists > settings.TriggerValue)
                    {
                        refBool.set(true);
                        refBoolVisitors.set(true);
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal_2", AutoDisableColonistShips.MESSAGE4), ResourceList.StaticIcons.Oxygen, 4));
                    }
                }
                else
                {
                    refBool.set(true);
                    refBoolVisitors.set(true);
                }
            }
        }
    }
}
