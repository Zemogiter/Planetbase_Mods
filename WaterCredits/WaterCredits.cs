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
        //divided the message because its a constant and constant does not allow variables (duh)
        public const string MESSAGE = "Sold ";
        public const string MESSAGE2 = " units of water for ";
        public const string MESSAGE3 = " coin(s) per unit.";
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
            if(GameManager.getInstance().getGameState() is GameStateGame && MyLandingShip.getState() == LandingShip.State.ClosingDoor)
            {
                WaterTransaction(Ship.mShips.First<Ship>() as LandingShip);
            }
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("message_water_transaction", MESSAGE);
            StringUtils.RegisterString("message_water_transaction_2", MESSAGE2);
            StringUtils.RegisterString("message_water_transaction_3", MESSAGE3);
            StringUtils.RegisterString("message_water_transaction_error", MESSAGE_ERROR);
            StringUtils.RegisterString("message_water_transaction_error_2", MESSAGE_ERROR2);
            StringUtils.RegisterString("message_water_transaction_error_3", MESSAGE_ERROR3);
        }
        public void WaterTransaction(LandingShip __instance)
        {
            //since we are working off of tanks, at least one tank is needed to execute the entire method correctly
            if (Module.getBuiltCountOfType(ModuleTypeList.find<ModuleTypeWaterTank>()) > 0)
            {
                //Module instance = (Module)Activator.CreateInstance(typeof(Module));
                int waterGeneration = CustomModule.getWaterGeneration();
                int waterStorage = Module.getOverallWaterStorage();
                if (waterGeneration >= WaterCredits.settings.minimumBalance && WaterCredits.settings.waterPerTransaction < waterStorage)
                {
                    float newWaterBalance = waterStorage - WaterCredits.settings.waterPerTransaction;
                    //typeof(Module).GetField("mWaterStorageIndicator", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(newWaterBalance);
                    CustomModule.mWaterStorageIndicator.setValue(newWaterBalance);
                    for (int i = 0; i == WaterCredits.settings.waterPerTransaction; i++)
                    {
                        ResourceType coinType = TypeList<ResourceType, ResourceTypeList>.find<Coins>();
                        for (int j = 0; j == WaterCredits.settings.coinsPerUnit; j++)
                        {
                            Resource.create(coinType, __instance.getPosition(), __instance.getLocation());
                        }
                    }
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_water_transaction", WaterCredits.MESSAGE + WaterCredits.settings.waterPerTransaction + WaterCredits.MESSAGE2 + WaterCredits.settings.coinsPerUnit + WaterCredits.MESSAGE3), ResourceList.StaticIcons.Water, 8));
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
    public class CustomModule : Module
    {
        public static new Indicator mWaterStorageIndicator;
        public static new int mCurrentWaterGeneration;
        public static new int getWaterGeneration()
        {
            return mCurrentWaterGeneration;
        }

    }
    public class CustomLandingShip : LandingShip
    {
        public State getState()
        {
            return mState;
        }
        public override GameObject getPrefab()
        {
            throw new NotImplementedException();
        }
    }
    public class MyLandingShip
    {
        public static LandingShip.State getState()
        {
            var state = new CustomLandingShip();
            return state.getState();
        }
    }
}
