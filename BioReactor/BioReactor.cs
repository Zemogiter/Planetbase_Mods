using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace BioReactor
{
    public class BioReactor : ModBase
    {
        public const string NAME = "BioReactor";
        public const string DESCRIPTION = "A dome fited with power cell arrays, designed to be powered by burning excess starch.";
        public static new void Init(ModEntry modEntry) => InitializeMod(new BioReactor(), modEntry, "BioReactor");

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
            RegisterNewBuilding();

            Debug.Log("[MOD] BioReactor activated");
        }
        private void RegisterNewBuilding()
        {
            var moduleList = ModuleTypeList.getInstance();

            moduleList.AddType(new ModuleTypeBioReactor());
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("bio_reactor", NAME);
            StringUtils.RegisterString("tooltip_bio_reactor", DESCRIPTION);
        }
       
        
        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing required here
        }
    }
    public static class ModuleGet
    {
        public static string ModuleTypeOxygenGeneratorName => Util.camelCaseToLowercase(TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeOxygenGenerator>().GetType().Name);
        public static GameObject GetOxygenGeneratorModel()
        {
            return ResourceUtil.loadPrefab(ModuleTypeOxygenGeneratorName);
        }
    }
    public class ModuleTypeBioReactor : ModuleType
    {
        public ModuleTypeBioReactor()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\BioReactor.png";
            //to be added
            //string texture = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\BioReactorTex.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            mPowerGeneration = 1;
            mWaterGeneration = -1000;
            mPowerStorageCapacity = 10000;
            mMinSize = 0;
            mMaxSize = 1;
            mFlags = 23498;
            mHeight = 0f;
            mLayoutType = LayoutType.Circular;
            mBaseType = false;
            mExterior = false;
            mModels = ModuleGet.GetOxygenGeneratorModel();
            mRequiredStructure.set<ModuleTypeOxygenGenerator>();
            mCost = new ResourceAmounts();
            mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 3);
            mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 3);
            mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
            mComponentTypes = new ComponentType[2]
            {
                TypeList<ComponentType, ComponentTypeList>.find<StarchBurner>(),
                TypeList<ComponentType, ComponentTypeList>.find<VegetableBurner>()
            };
            initStrings();
        }
    }
    public class StarchBurner : ComponentType
    {
        public const string Name = "StarchBurner";
        public const string Description = "A starch burning furnance, then collects thermal energy to charge BioReactor's cells.";

        public StarchBurner()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\StarchBurner.png";
            //to be added
            //string texture = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\StarchBurnerTex.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
            mResourceConsumption = new List<ResourceType>
            {
                TypeList<ResourceType, ResourceTypeList>.find<Starch>()
            };
            mEmbeddedResourceCount = 2;
            mPowerGeneration = +2000;
            mWaterGeneration = -500;
            mResourceProductionPeriod = 1f;
            mFlags = 1248510;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mPrefabName = "PrefabBioplasticProcessor";
            addUsageAnimation(CharacterAnimationType.Watch);
            initStrings();
        }
        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starch_burner", Name);
            StringUtils.RegisterString("tooltip_starch_burner", Description);
        }
    }
    public class VegetableBurner : ComponentType
    {
        public const string Name = "VegetableBurner";
        public const string Description = "A vegetable burning furnance, then collects thermal energy to charge BioReactor's cells. Last Resort Option!";

        public VegetableBurner()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\VegetableBurner.png";
            //to be added
            //string texture = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\VegetableBurnerTex.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
            mResourceConsumption = new List<ResourceType>
            {
                TypeList<ResourceType, ResourceTypeList>.find<Vegetables>()
            };
            mEmbeddedResourceCount = 2;
            mPowerGeneration = +2000;
            mWaterGeneration = -500;
            mResourceProductionPeriod = 1f;
            mFlags = 1028519;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mPrefabName = "PrefabBioplasticProcessor";
            addUsageAnimation(CharacterAnimationType.Watch);
            initStrings();
        }
        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_vegetable_burner", Name);
            StringUtils.RegisterString("tooltip_vegetable_burner", Description);
        }
    }
}
