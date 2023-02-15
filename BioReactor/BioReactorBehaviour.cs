using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioReactor
{
    internal class BioReactorBehaviour
    {
        public void BioReactorPowerGeneration()
        {
            var originalList = BuildableUtils.GetAllComponents();
            var burnerType = BuildableUtils.FindComponentType<StarchBurner>();
            var burnerType2 = BuildableUtils.FindComponentType<VegetableBurner>();
            var bioreactorType = BuildableUtils.FindModuleType<ModuleTypeBioReactor>();
            var burnerList = originalList.Where(a => a.getComponentType() == burnerType || a.getComponentType() == burnerType2).ToList();

            foreach(var component in burnerList)
            {
                if (component.isOperational())
                {
                    //To-do: figure out if we should add extra power generation to the BioReactor itself or to global production generation
                    bioreactorType.mPowerGeneration += BioReactor.settings.burnerPowerGeneration;
                    bioreactorType.mWaterGeneration += BioReactor.settings.burnerWaterConsumption * -1;
                }
            }
        }
        public static ResourceType getPlaceholder()
        {
            return ResourceTypeList.CoinsInstance as ResourceType;
        }
    }
}
