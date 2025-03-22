﻿using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityModManagerNet;

namespace Deliveries
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Cheat mode (deliveries are free)")] public bool cheatMode = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class Deliveries : ModBase
    {
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

            InitializeMod(new Deliveries(), modEntry, "Deliveries");
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

        public const string DeliveryLabel = "Send a delivery.";
        public const string DeliveryErrorMessage = "A landing pad of any kind is needed.";
        public const string DeliveryErrorMessage2 = "Not enough coins to fund the delivery costs.";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if (!ActiveDeliveryShip) return;

            //Wait for a ship to land and be recycled before sending another
            if (Ship.isDone() && Ship.getGameObject() == null)
            {
                ActiveDeliveryShip = false;
            }
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("menu_deliver", DeliveryLabel);
            StringUtils.RegisterString("delivery_error", DeliveryErrorMessage);
        }
    }
}
