using HarmonyLib;
using Planetbase;
using ExtensionMethods;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSpeedDisplay
{
    /// <summary>
	/// Patch GuiGeneralPanel update method to replace the top-left panel with our improved window
	/// </summary>
    [HarmonyPatch(typeof(GuiGeneralPanel), nameof(GuiGeneralPanel.update))]
    public class GuiGeneralPanelPatch
    {

        public static string mTimeScaleCountString;
        public static bool Prefix(float timeStep, GuiGeneralPanel __instance)
        {
            __instance.mTimeSinceUpdate -= timeStep;
            if (__instance.mTimeSinceUpdate < 0f)
            {
                __instance.mColonistCountString = Singleton<Colony>.getInstance().getColonistCount().ToString();
                __instance.mBotCountString = Character.getCountOfType<Bot>().ToString();
                mTimeScaleCountString = Singleton<TimeManager>.getInstance().getTimeScale().ToString();
                __instance.mTimeSinceUpdate = 1f;
            }
            return false;

        }
        public static void Postfix(ref GuiGeneralPanel __instance)
        {
            StringExtensions.getTimeScaleCountString(__instance);
        }
        
    }
    
}
