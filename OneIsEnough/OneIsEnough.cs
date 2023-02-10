using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using System.Reflection;
using Module = Planetbase.Module;

namespace OneIsEnough
{
    public class OneIsEnough : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new OneIsEnough(), modEntry, "OneIsEnough");

        public float[] singleNumber = new float[2] { 0.9f, 1f };
        BindingFlags flags = BindingFlags.NonPublic| BindingFlags.Instance;
        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleName telescope = ModuleName.Telescope;
            typeof(ModuleTypeTelescope)
                .GetField("mDisasterInterceptionChances", flags)
                .SetValue(telescope, singleNumber);
            ModuleName radio = ModuleName.RadioAntenna;
            typeof(ModuleTypeRadioAntenna)
                .GetField("mRadioInterceptionChances", flags)
                .SetValue(radio, singleNumber);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            /*if (GameManager.getInstance().getGameState() is GameStateGame)
            {
                if (Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeTelescope>()) > 0)
                {
                    ModuleName telescope = ModuleName.Telescope;
                    typeof(ModuleType).GetField("mDisasterInterceptionChances", flags).SetValue(telescope, singleNumber);
                }
                if (Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeRadioAntenna>()) > 0)
                {
                    ModuleName radio = ModuleName.RadioAntenna;
                    typeof(ModuleType).GetField("mRadioInterceptionChances", flags).SetValue(radio, singleNumber);
                }
            }*/
        }
    }
    
    /*[HarmonyPatch(typeof(Module), nameof(Module.calculateRadioInterceptionChance))]
    public class ChancePatch
    {
        public static float Postfix(float num, Module __instance)
        {
            if (Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeRadioAntenna>()) > 0 && __instance.isUnoperated() == false)
            {
                return num = 1f;
            }
            return num;
        }

    }
    [HarmonyPatch(typeof(Module), nameof(Module.calculateDisasterInterceptionChance))]
    public class ChancePatch2
    {
        
        public static float Postfix(float num, Module __instance)
        {
            if (Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeTelescope>()) > 0 && __instance.isUnoperated() == false)
            {
                return num = 1f;
            }
            return num;
        }
    }*/
    
}
