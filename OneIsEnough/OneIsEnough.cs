using System.Linq;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using Module = Planetbase.Module;

namespace OneIsEnough
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Allow sandstorms and blizzards to interfere with telescopes?")] public bool AllowWeatherInterference = false;
        [Draw("Storm debuff number for telescopes (the bigger the less effective telescopes are during extreme weather conditions")] public float StormDebuff = 0.5f;
        [Draw("Allow solar flares to interfere with radios?")] public bool AllowSolarFlareInteference = false;
        [Draw("Solar flare debuff number for radios (the bigger the less effective radios are during solar flares")] public float SolarFlareDebuff = 0.5f;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class OneIsEnough : ModBase
    {
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new OneIsEnough(), modEntry, "OneIsEnough");
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
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    [HarmonyPatch(typeof(Module), nameof(Module.calculateRadioInterceptionChance))]
    public class ModuleRadioPatch
    {
        static float Postfix(float num)
        {
            num = 0f;
            var originalList = BuildableUtils.GetAllModules();
            var radioType = BuildableUtils.FindModuleType<ModuleTypeRadioAntenna>();
            var radioList = originalList.Where(a => a.getModuleType() == radioType).ToList();
           
            foreach(Module radio in radioList) 
            {
            
                if(radio.isOperational() && !radio.isUnoperated())
                {
                    float radioInterceptionChance = 1f;
                    num += (1f - num) * radioInterceptionChance;
                    if (OneIsEnough.settings.AllowSolarFlareInteference && DisasterManager.getInstance().getSolarFlare() != null)
                    {
                        num += (1f - num) * radioInterceptionChance - OneIsEnough.settings.SolarFlareDebuff;
                    }
                }
            }
            return num;
        }

    }
    [HarmonyPatch(typeof(Module), nameof(Module.calculateDisasterInterceptionChance))]
    public class ModuleTelescopePatch
    {
        static float Postfix(float num)
        {
            num = 0f;
            var originalList = BuildableUtils.GetAllModules();
            var telescopeType = BuildableUtils.FindModuleType<ModuleTypeTelescope>();
            var telescopeList = originalList.Where(a => a.getModuleType() == telescopeType).ToList();
            foreach (Module telescope in telescopeList)
            {
                if (telescope.isOperational() && !telescope.isUnoperated())
                {
                    float disasterInterceptionChance = 1f;
                    num += (1f - num) * disasterInterceptionChance;
                    if (OneIsEnough.settings.AllowWeatherInterference && DisasterManager.getInstance().getStormInProgress() != null)
                    {
                        num += (1f - num) * disasterInterceptionChance - OneIsEnough.settings.StormDebuff;
                    }
                }
            }
            return num;
        }
    }
}
