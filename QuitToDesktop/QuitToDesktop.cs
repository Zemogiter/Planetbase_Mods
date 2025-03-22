using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace QuitToDesktop
{
    public class QuitToDesktop : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new QuitToDesktop(), modEntry, "QuitToDesktop");
        public static string MESSAGE = "Quit to desktop";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("button_quit", MESSAGE);
        }
    }
    [HarmonyPatch(typeof(GuiGameMenu), nameof(GuiGameMenu.init))]
    public class PauseMenuPatch
    {
        public static void OnQuitPM(object parameter)
        {
            Debug.Log("Application quitting from the pause menu");
            Application.Quit();
        }
        public static void Postfix(GuiGameMenu __instance)
        {
            GuiDefinitions.Callback callbackQuit = new(OnQuitPM);
            __instance.addButton("button_quit", callbackQuit, true);
        }
    }
}
