using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BetterHours
{
    public class BetterHours : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new BetterHours(), modEntry, "BetterHours");

		public override void OnInitialized(ModEntry modEntry)
		{
            
        }

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            if (GameManager.getInstance().getGameState() is GameStateGame)
            {
                float value = (float)((Singleton<EnvironmentManager>.getInstance().getDayTime() + Singleton<EnvironmentManager>.getInstance().getNightTime()) / (BetterHours.GetDayHours() / 6.0));
                Traverse.Create(StatsCollector.getInstance()).Field("m_MinFallingHeight").SetValue(value);
            }
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
    [HarmonyPatch(typeof(Indicator), nameof(Indicator.getTimeHour))]
    public class IndicatorPatch
    {
        static int Postfix(int value)
        {
            double dayHours = BetterHours.GetDayHours();
            return (int)((value * dayHours + 6.0) % dayHours);
        }
    }
}
