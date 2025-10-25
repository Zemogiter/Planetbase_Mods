using System;
using Planetbase;
using HarmonyLib;
using PlanetbaseModUtilities;

namespace Deliveries.Patches
{
    [HarmonyPatch(typeof(Module), nameof(Module.update))]
    public class ModulePatches
    {
        public static void Postfix(Module __instance)
        {
            //patch that checks if a ColonyShip is occupying a landing pad/starport and prevents other ships from landing on it
            foreach (Module module in BuildableUtils.GetAllModules())
            {
                if ((module.getModuleType() == TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeLandingPad>() || module.getModuleType() == TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStarport>()) && (ButtonContainer.LandingCheck().getPosition().x == module.getPosition().x && ButtonContainer.LandingCheck().getPosition().z == module.getPosition().z) && Deliveries.ActiveDeliveryShip == true && Module.getOverallPowerStorage() > 0 && Module.getOverallPowerBalance() > 0)
                {
                    Module targetModule = module;
                    targetModule.setEnabled(false);
                    //if (Deliveries.settings.debugMode) Console.WriteLine("Deliveries - Landing pad/starport is occupied by a ship.");
                    if (Deliveries.ActiveDeliveryShip == false)
                    {
                        targetModule.setEnabled(true);
                        //if (Deliveries.settings.debugMode) Console.WriteLine("Deliveries - Landing pad/starport is free.");
                    }
                }
            }

        }
    }
}
