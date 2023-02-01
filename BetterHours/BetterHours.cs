using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using System.Reflection;

namespace BetterHours
{
    public class BetterHours : ModBase
    {
        public static float timeStep;
        public static new void Init(ModEntry modEntry) => InitializeMod(new BetterHours(), modEntry, "BetterHours");

		public override void OnInitialized(ModEntry modEntry)
		{
            
		}

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            
        }

        public static double GetDayHours()
        {
            double dayHours = 24.0;
            PlanetManager pManager = Singleton<PlanetManager>.getInstance();
            if (pManager != null && pManager.mCurrentPlanet != null && pManager.mCurrentPlanet.mDefinition != null)
            {
                dayHours = pManager.mCurrentPlanet.mDefinition.DayHours + pManager.mCurrentPlanet.mDefinition.NightHours;
            }
            return dayHours;
        }
    }
    [HarmonyPatch(typeof(StatsCollector), nameof(StatsCollector.mRefreshPeriod))]
    public class StatsCollectorPatch
    {
        static void Postfix(StatsCollector __instance)
        {
            PropertyInfo refreshPeriod = __instance.GetType().GetProperty("mRefreshPeriod");
            //__instance.mRefreshPeriod = (float)((Singleton<EnvironmentManager>.getInstance().getDayTime() + Singleton<EnvironmentManager>.getInstance().getNightTime()) / (BetterHours.GetDayHours() / 6.0));
            float value = (float)((Singleton<EnvironmentManager>.getInstance().getDayTime() + Singleton<EnvironmentManager>.getInstance().getNightTime()) / (BetterHours.GetDayHours() / 6.0));
            refreshPeriod.SetValue(__instance,value);
        }
    }
    [HarmonyPatch(typeof(Indicator), nameof(Indicator.getTimeHour))]
    public class IndicatorPatch
    {
        public static int Prefix(float value)
        {
            double dayHours = BetterHours.GetDayHours();
            return (int)(((double)value * dayHours + 6.0) % dayHours);
        }
    }
}
