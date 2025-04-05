using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using Module = Planetbase.Module;
using UnityEngine.Profiling;

namespace PrestigeMod
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Resource calculation number (smaller number = more prestige). Game default is 3.")] public int resCalcNumber = 3;
        [Draw("Should we add a colony's total coins to prestige calculations?")] public bool includeCoins = true;
        //[Draw("Should we add a colony's total bots to prestige calculations?")] public bool includeBots = true;
        [Draw("Debug mode")] public bool debugMode = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class PrestigeMod : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new PrestigeMod(), modEntry, "PrestigeMod");
            Harmony harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
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

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //moddifies the number the toal resource amount is divided by (smaller number = bigger result) and includes current coin amount to prestige calculations
            if (GameManager.getInstance().getGameState() is GameStateGame)
            {
                var colonyInstance = Singleton<Colony>.getInstance();
                int num = Mathf.Min(Module.calculateTotalPrestige(), 300);
                int num2 = Mathf.Min(Character.getCount(), 300);
                int num3 = Mathf.Min(Resource.getCount() / PrestigeMod.settings.resCalcNumber, 200);
                if (PrestigeMod.settings.debugMode) Console.WriteLine("Values of num num2 num3: " + num.ToString() + " " + num2.ToString() + " " + num3.ToString());
                int extraPrestige = CoreUtils.GetMember<Colony, int>("mExtraPrestige", colonyInstance);
                int num4 = Mathf.Min(extraPrestige, 200);
                int num5 = Resource.getCountOfType(TypeList<ResourceType, ResourceTypeList>.find<Coins>()) / num2;
                if (PrestigeMod.settings.debugMode) Console.WriteLine("Values of extraPrestige num4 num5: " + extraPrestige.ToString() + " " + num4.ToString() + " " + num5.ToString());
                var prestigeIndicator = CoreUtils.GetMember<Colony, Indicator>("mPrestigeIndicator", colonyInstance);

                if (PrestigeMod.settings.debugMode) Console.WriteLine("Value of prestige indicator" + prestigeIndicator.getLevel().ToString() + " " + prestigeIndicator.getLevels().ToString() + " " + prestigeIndicator.getValue().ToString());

                if (PrestigeMod.settings.includeCoins == false)
                {
                    prestigeIndicator.setValue(num + num2 + num3 + num + num4);
                    if (PrestigeMod.settings.debugMode) Console.WriteLine("Prestige after changes (not including coins): " + prestigeIndicator.getValue().ToString());
                }
                else
                {
                    prestigeIndicator.setValue(num + num2 + num3 + num + num4 + num5);
                    if (PrestigeMod.settings.debugMode) Console.WriteLine("Prestige after changes (including coins): " + prestigeIndicator.getValue().ToString());
                }
                if (colonyInstance.getPrestigeIndicator().getValue() > 1000)
                {
                    colonyInstance.getPrestigeIndicator().setValue(1000);
                    if (PrestigeMod.settings.debugMode) Console.WriteLine("Prestige capped at 1000");
                }
            }
        }
    }
}
