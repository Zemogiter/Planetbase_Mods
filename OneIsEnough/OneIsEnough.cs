using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using static UnityEngine.UI.InputField;
using static UnityModManagerNet.UnityModManager;
using Module = Planetbase.Module;

namespace OneIsEnough
{
    public class OneIsEnough : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new OneIsEnough(), modEntry, "OneIsEnough");

        public float[] betterChances = new float[2] { 0.9f, 1f };
        BindingFlags flags = BindingFlags.NonPublic| BindingFlags.Instance;
        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleName telescope = ModuleName.Telescope;
            typeof(ModuleTypeTelescope)
                .GetField("mDisasterInterceptionChances", flags)
                .SetValue(telescope, betterChances);
            ModuleName radio = ModuleName.RadioAntenna;
            typeof(ModuleTypeRadioAntenna)
                .GetField("mRadioInterceptionChances", flags)
                .SetValue(radio, betterChances);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
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
                    float disasterInterceptionChance = 1f;
                    num += (1f - num) * disasterInterceptionChance;
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
                    float radioInterceptionChance = 1f;
                    num += (1f - num) * radioInterceptionChance;
                }
            }
            return num;
        }
    }
}
