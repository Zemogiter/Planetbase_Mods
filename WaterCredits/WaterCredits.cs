using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using UnityEngine.UI;
using HarmonyLib;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using Module = Planetbase.Module;

namespace WaterCredits
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Coins per unit of water sold")] public int coinsPerUnit = 1;
        [Draw("Minimum water balance(no transaction if balance is below this number)")] public int minimumBalance = 2;
        [Draw("Water ammount sold per transaction")] public int waterPerTransaction = 100;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class WaterCredits : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public const string MESSAGE_ERROR = "Water balance is below minimum for trading.";
        public const string MESSAGE_ERROR2 = "The value of waterPerTransaction is bigger than the current storage";
        public const string MESSAGE_ERROR3 = "To trade water with departing ships, build a water tank.";
        public static new void Init(ModEntry modEntry) 
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new WaterCredits(), modEntry, "WaterCredits"); 
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

        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("message_water_transaction", GetMessageContent());
            StringUtils.RegisterString("message_water_transaction_error", MESSAGE_ERROR);
            StringUtils.RegisterString("message_water_transaction_error_2", MESSAGE_ERROR2);
            StringUtils.RegisterString("message_water_transaction_error_3", MESSAGE_ERROR3);
        }
        public static string GetMessageContent()
        {
            return $"Sold {WaterCredits.settings.waterPerTransaction} units of water for {WaterCredits.settings.coinsPerUnit} coin(s) per unit.";
        }
        public static int GetWaterCreditsCount()
        {
            int count = WaterCredits.settings.coinsPerUnit * WaterCredits.settings.waterPerTransaction;
            Console.WriteLine(count);
            return count;
        }
        public static void RemoveSoldWater()
        {
            int waterStorage = Module.getOverallWaterStorage();
            int newWaterStorage = waterStorage - WaterCredits.settings.waterPerTransaction;
            //To-do: set the water storage variable to newWaterStorage
        }
    }
    [HarmonyPatch(typeof(LandingShip), nameof(LandingShip.onClosingDoor))]
    public class LandingShipPatch
    {
        static void Postfix(LandingShip __instance)
        {
            //since we are working off of tanks, at least one tank is needed to execute the entire method correctly
            if (Module.getBuiltCountOfType(ModuleTypeList.find<ModuleTypeWaterTank>()) > 0)
            {
                int waterGeneration = Module.getOverallWaterBalance();
                int waterStorage = Module.getOverallWaterStorage();
                //Console.WriteLine("Water storage is " + waterStorage);
                if (waterGeneration > WaterCredits.settings.minimumBalance && WaterCredits.settings.waterPerTransaction < waterStorage)
                {
                    //WaterCredits.RemoveSoldWater();
                    ResourceType coinType = TypeList<ResourceType, ResourceTypeList>.find<Coins>();
                    //var coinType = ResourceTypeList.CoinsInstance;
                    for (int i = 0; i == WaterCredits.GetWaterCreditsCount(); i++)
                    {
                        Console.WriteLine("Position of the ship/coin " + __instance.getPosition());
                        Resource.create(coinType, __instance.getPosition(), __instance.getLocation());
                    }
                    Message message = new (StringList.get("message_water_transaction", WaterCredits.GetMessageContent()), ResourceList.StaticIcons.Water, 8);
                    Singleton<MessageLog>.getInstance().addMessage(message);
                }
                else if (waterGeneration <= WaterCredits.settings.minimumBalance)
                {
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_water_transaction_error", WaterCredits.MESSAGE_ERROR), ResourceList.StaticIcons.Water, 8));
                }
                else
                {
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_water_transaction_error_2", WaterCredits.MESSAGE_ERROR2), ResourceList.StaticIcons.Water, 8));
                }
            }
            else
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_water_transaction_error_3", WaterCredits.MESSAGE_ERROR3), ResourceList.StaticIcons.Water, 8));
            }
        }
    }
}
