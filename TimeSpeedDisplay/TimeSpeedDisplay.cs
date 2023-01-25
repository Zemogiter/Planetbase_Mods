using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;

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
}
