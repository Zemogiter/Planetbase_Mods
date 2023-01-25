using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Diagnostics;

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
            try
            {
                StatsCollector sCollector = Singleton<StatsCollector>.getInstance();
                //Debug.WriteLine("This is the value of sCollector " + sCollector);
                EnvironmentManager eManager = Singleton<EnvironmentManager>.getInstance();
                //Debug.WriteLine("This is the value of eManager " + eManager);
                if (sCollector != null && eManager != null)
                {
                    double dayHours = GetDayHours();
                    sCollector.mRefreshPeriod = (float)(((double)eManager.getDayTime() + (double)eManager.getNightTime()) / (dayHours / 6.0));
                };
            }
            catch (NullReferenceException exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        public double GetDayHours()
        {
            double dayHours = 24.0;
            PlanetManager pManager = Singleton<PlanetManager>.getInstance();
            if (pManager != null && pManager.mCurrentPlanet != null && pManager.mCurrentPlanet.mDefinition != null)
            {
                dayHours = pManager.mCurrentPlanet.mDefinition.DayHours + pManager.mCurrentPlanet.mDefinition.NightHours;
            }
            return dayHours;
        }
        public int GetTimeHour(float value)
        {
            double dayHours = GetDayHours();
            return (int)(((double)value * dayHours + 6.0) % dayHours);
        }
    }
}
