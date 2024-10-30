using System.IO;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace ExteriorO2Genereator
{
    public class ExteriorO2Genereator : ModBase
    {
        public const string NAME = "Exterior O2 Generator";
        public const string DESCRIPTION = "A variant of oxygen generation that isn't a walkable dome. Good for oxygenating the edges of any base.";

        public static new void Init(ModEntry modEntry) => InitializeMod(new ExteriorO2Genereator(), modEntry, "ExteriorO2Genereator");

        public override void OnInitialized(ModEntry modEntry)
		{
            RegisterStrings();
            RegisterNewBuilding();

            Debug.Log("[MOD] ExteriorO2Genereator activated");
        }

        private static void RegisterStrings()
        {
            StringUtils.RegisterString("exterior_oxygen_generator", NAME);
            StringUtils.RegisterString("tooltip_exterior_oxygen_generator", DESCRIPTION);
        }
        private void RegisterNewBuilding()
        {
            var moduleList = ModuleTypeList.getInstance();

            moduleList.AddType(new ModuleTypeExteriorOxygenGenerator());
        }
        public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            // Nothing required here
        }
    }
    public class ModuleTypeExteriorOxygenGenerator : ModuleType
	{
        public GameObject model;
        public ModuleTypeExteriorOxygenGenerator()
		{
            string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\ExteriorO2Generator\\Textures\\ExteriorO2Generator.png";
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            else
            {
                this.mIcon = this.mIcon = ResourceList.StaticIcons.Oxygen;
            }
            mPowerGeneration = -1000;
			mWaterGeneration = -1000;
			mMinSize = 0;
			mMaxSize = 0;
			mOxygenGeneration = 20;
			mFlags = 55000;
			mHeight = 0f;
            mBaseType = true;
            mExterior = true;
            mModels[0] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabBasePad1");
            mRequiredStructure.set<ModuleTypeOxygenGenerator>();
			mCost = new ResourceAmounts();
            mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 2);
			mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 2);
            initStrings();
        }
	}
}
