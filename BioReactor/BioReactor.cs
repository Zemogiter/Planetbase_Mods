using System.Diagnostics.CodeAnalysis;
using System.IO;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace BioReactor
{
    public class Settings : ModSettings, IDrawable
    {
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        [Draw("Use Custom Icon")] public bool UseCustomIcon = false;
        [Draw("Power generation per burner")] public int BurnerPowerGeneration = 1000;
        [Draw("Water consumption per burner")] public int BurnerWaterConsumption = 2000; 
        
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class BioReactor : ModBase
    {
        public const string Name = "Bio Reactor";
        public const string Description = "A dome fited with power cell arrays, designed to be powered by burning excess starch and other organic matter.";

        public static bool enabled;
        public static Settings settings;

        public new static void Init(ModEntry modEntry)
        {
            settings = ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new BioReactor(), modEntry, "BioReactor");
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
            //For future reference, adding new buildings must be done in this order or else the mod fails to load or game crashes when clicking on the "edit module" button:
            RegisterStrings();
            RegisterNewComponents();
            RegisterNewBuilding();
            AddNewComponentsToBuilding();

            Debug.Log("[MOD] BioReactor activated");
        }
        private void RegisterNewBuilding()
        {
            var moduleList = ModuleTypeList.getInstance();

            moduleList.AddType(new ModuleTypeBioReactor());
        }
        private void RegisterNewComponents()
        {
            // Register new components to global lists
            var componentTypeList = ComponentTypeList.getInstance();

            componentTypeList.AddType(new StarchBurner());
            componentTypeList.AddType(new VegetableBurner());
        }
        private void AddNewComponentsToBuilding()
        {
            // Add new components to bioreactor
            var bioReactorComponents = BuildableUtils.FindModuleType<ModuleTypeBioReactor>().GetComponentTypes();

            bioReactorComponents.Add(ComponentTypeList.find<StarchBurner>());
            bioReactorComponents.Add(ComponentTypeList.find<VegetableBurner>());
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("bio_reactor", Name);
            StringUtils.RegisterString("tooltip_bio_reactor", Description);
            StarchBurner.RegisterStrings();
            VegetableBurner.RegisterStrings();
        }
       
        
        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if (GameManager.getInstance().getGameState() is GameStateGame)
            {
                BioReactorBehaviour.BioReactorPowerGeneration();
            }
        }
    }
    public class ModuleTypeBioReactor : ModuleType
    {
        public ModuleTypeBioReactor()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\BioReactor.png";
            if (File.Exists(path) && BioReactor.settings.UseCustomIcon)
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadRawTextureData(iconBytes);
                mIcon = Util.applyColor(tex);
            }
            else
            {
                mIcon = ResourceList.StaticIcons.PowerGrid;
            }
            mWaterGeneration = -1000;
            mPowerStorageCapacity = 5000000;
            mPowerGeneration = 0;
            mMinSize = 1;
            mMaxSize = 1;
            mFlags = 1050000;
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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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
                tex.LoadRawTextureData(iconBytes);
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
            mResourceConsumption = [TypeList<ResourceType, ResourceTypeList>.find<Starch>()];
            addResourceProduction<Coins>();
            mEmbeddedResourceCount = 3;
            mResourceProductionPeriod = 1f;
            mPowerGeneration = 30000;
            mWaterGeneration = BioReactor.settings.BurnerWaterConsumption * -1;
            mFlags = 148510;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mPrefabName = "PrefabBioplasticProcessor";
            addUsageAnimation(CharacterAnimationType.WorkStanding);
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
        public const string Name = "Vegetable Burner";
        public const string Description = "A vegetable burning furnance, then collects thermal energy to charge BioReactor's cells. Last Resort Option!";

        public VegetableBurner()
        {
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\BioReactor\\Textures\\VegetableBurner.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadRawTextureData(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            else
            {
                this.mIcon = LoadIcon();
            }
            mConstructionCosts = new ResourceAmounts();
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 1);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);
            mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Semiconductors>(), 1);
            mResourceConsumption = [TypeList<ResourceType, ResourceTypeList>.find<Vegetables>()];
            addResourceProduction<Coins>();
            mEmbeddedResourceCount = 2;
            mResourceProductionPeriod = 1f;
            mPowerGeneration = 30000;
            mWaterGeneration = BioReactor.settings.BurnerWaterConsumption * -1;
            mFlags = 108519;
            mOperatorSpecialization = TypeList<Specialization, SpecializationList>.find<Worker>();
            mPrefabName = "PrefabBioplasticProcessor";
            addUsageAnimation(CharacterAnimationType.WorkStanding);
            initStrings();
        }
        public Texture2D LoadIcon()
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
