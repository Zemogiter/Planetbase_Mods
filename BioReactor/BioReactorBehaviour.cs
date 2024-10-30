using PlanetbaseModUtilities;

namespace BioReactor
{
    internal class BioReactorBehaviour
    {
        public static void BioReactorPowerGeneration()
        {
            var originalList = BuildableUtils.GetAllComponents();
            var burnerType = BuildableUtils.FindComponentType<StarchBurner>();
            var burnerType2 = BuildableUtils.FindComponentType<VegetableBurner>();
            var bioreactorType = BuildableUtils.FindModuleType<ModuleTypeBioReactor>();
            var burnerList = originalList.Find(a =>
                a.getComponentType() == burnerType || a.getComponentType() == burnerType2);

            foreach(var component in burnerList)
            {
                if (component.isOperational())
                {
                    //To-do: figure out if we should add extra power generation to the BioReactor itself or to global production generation
                }
            }
        }
    }
}
