using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Object = UnityEngine.Object;

namespace BioReactor
{
    public class BioReactor : ModBase
    {
        public const string NAME = "Bio Reactor";
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
    public class ModuleTypeBioReactor : ModuleType
    {
        public ModuleTypeBioReactor()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\BioReactor.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex, GuiStyles.UiColorMain);
            }
            else
            {
                this.mIcon = ResourceList.StaticIcons.PowerGrid;
            }
            mWaterGeneration = -1000;
            mPowerStorageCapacity = 5000000;
            mMinSize = 1;
            mMaxSize = 1;
            mFlags = 1050660;
            mLayoutType = LayoutType.Circular;
            mModels[1] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabOxygenGenerator2");
            mRequiredStructure.set<ModuleTypePowerCollector>();
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
        public const string Name = "Starch Burner";
        public const string Description = "A starch burning furnance, then collects thermal energy to charge BioReactor's cells.";

        public StarchBurner()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\StarchBurner.png";
            
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            else
            {
                this.mIcon = ResourceUtil.loadIconColor("Components/icon_" + Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<BioplasticProcessor>().GetType().Name));
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
            mResourceProductionPeriod = 1f;
            mFlags = 1248510;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mPrefabName = "PrefabBioplasticProcessor";
            addUsageAnimation(CharacterAnimationType.WorkStanding, CharacterProp.Count, CharacterProp.Count);
            initStrings();
        }
        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starch_burner", Name);
            StringUtils.RegisterString("tooltip_starch_burner", Description);
        }
        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.BioplasticProcessorTypeName);
        }
    }
    public class VegetableBurner : ComponentType
    {
        public const string Name = "Vegetable Burner";
        public const string Description = "A vegetable burning furnance, then collects thermal energy to charge BioReactor's cells. Last Resort Option!";

        public VegetableBurner()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\VegetableBurner.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            else
            {
                this.mIcon = loadIcon();
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
            mResourceProductionPeriod = 1f;
            mFlags = 108519;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mPrefabName = "PrefabBioplasticProcessor";
            addUsageAnimation(CharacterAnimationType.WorkStanding, CharacterProp.Count, CharacterProp.Count);
            initStrings();
        }
        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.BioplasticProcessorTypeName);
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_vegetable_burner", Name);
            StringUtils.RegisterString("tooltip_vegetable_burner", Description);
        }
    }
}
