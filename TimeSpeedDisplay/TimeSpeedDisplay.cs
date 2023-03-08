using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;

namespace TimeSpeedDisplay
{
    public class TimeSpeedDisplay : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new TimeSpeedDisplay(), modEntry, "TimeSpeedDisplay");

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing required here
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("current_game_speed", "Current Game Speed");
        }
    }
    public class CustomGeneralPanel : GuiGeneralPanel
    {
        public string mCurrentTimeScaleString;

        public new void update(float timeStep)
        {
            mTimeSinceUpdate -= timeStep;
            if (mTimeSinceUpdate < 0f)
            {
                mColonistCountString = Singleton<Colony>.getInstance().getColonistCount().ToString();
                mBotCountString = Character.getCountOfType<Bot>().ToString();
                mCurrentTimeScaleString = Singleton<TimeManager>.getInstance().getTimeScale().ToString();
                mTimeSinceUpdate = 1f;
            }
        }
        public string GetCurrentTimeScaleString()
        {
            return mCurrentTimeScaleString;
        }
        public float GetTimeSinceUpdate()
        {
            return mTimeSinceUpdate;
        }
    }
    /*[HarmonyPatch(typeof(GuiGeneralPanel), nameof(GuiGeneralPanel.update))]
    public class GuiGeneralPanelPatch : CustomGeneralPanel
    {
        static bool Prefix(InteractionAirlock __instance, ref bool __result, float timeStep)
        {
            __result = ReplacementMethod(__instance, timeStep);
            return false;
        }
        public static bool ReplacementMethod(InteractionAirlock __instance, float timeStep)
        {
            mTimeSinceUpdate -= timeStep;
            if (mTimeSinceUpdate < 0f)
            {
                mColonistCountString = Singleton<Colony>.getInstance().getColonistCount().ToString();
                mBotCountString = Character.getCountOfType<Bot>().ToString();
                mCurrentTimeScaleString = Singleton<TimeManager>.getInstance().getTimeScale().ToString();
                mTimeSinceUpdate = 1f;
            }
            return false;
        }
    }*/
}
