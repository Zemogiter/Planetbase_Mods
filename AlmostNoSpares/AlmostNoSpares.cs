using System.Reflection;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace AlmostNoSpares
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("New decay time for solar panels")] public readonly float SolarPanelDecayTime = 14000f;
        [Draw("New decay time for wind turbines")] public readonly float WindTurbineDecayTime = 14000f;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class AlmostNoSpares : ModBase
    {
        public static bool enabled;
        // ReSharper disable once MemberCanBePrivate.Global
        public static Settings settings;
        public new static void Init(ModEntry modEntry)
        {
            settings = ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new AlmostNoSpares(), modEntry, "AlmostNoSpares");
        }
        static void OnGUI(ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleType solar = new ModuleTypeSolarPanel();
            typeof(ModuleTypeSolarPanel)
                .GetField("mCondicionDecayTime", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(solar, settings.SolarPanelDecayTime);
            ModuleType wind = new ModuleTypeWindTurbine();
            typeof(ModuleTypeWindTurbine)
                .GetField("mCondicionDecayTime", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(wind, settings.WindTurbineDecayTime);
        }
        
        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
}
