using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Reflection;
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
            // Poll for a pending quit request and perform Application.Quit from Update (safe context).
            if (PauseMenuPatch.RequestQuit)
            {
                PauseMenuPatch.RequestQuit = false;
                PauseMenuPatch.PerformQuit();
            }
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("button_quit", MESSAGE);
        }
    }
    [HarmonyPatch(typeof(GuiGameMenu), nameof(GuiGameMenu.init))]
    public class PauseMenuPatch
    {
        // Set when user clicks quit in the GUI. Do not call Application.Quit directly from GUI rendering code.
        public static volatile bool RequestQuit = false;

        public static void OnQuitPM(object parameter)
        {
            Debug.Log("Quit button pressed — scheduling application quit on next Update");
            RequestQuit = true;
        }

        // Called from QuitToDesktop.OnUpdate to execute Application.Quit from a safer context.
        public static void PerformQuit()
        {
            Debug.Log("Performing scheduled Application.Quit()");
            try
            {
                Application.Quit();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                try { Application.Quit(); } catch { /* swallow */ }
            }
        }

        public static void Postfix(GuiGameMenu __instance)
        {
            GuiDefinitions.Callback callbackQuit = new(OnQuitPM);
            __instance.addButton("button_quit", callbackQuit, true);
        }
    }
    // Suppress the original GameManager.onQuit to avoid invoking game shutdown paths that trigger native crashes.
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.onQuit))]
    public class GameManagerOnQuitPatch
    {
        public static bool Prefix(GameManager __instance)
        {
            Debug.Log("Patched GameManager.onQuit suppressed to avoid unsafe shutdown code.");
            // Skip original onQuit entirely.
            return false;
        }
    }
}
