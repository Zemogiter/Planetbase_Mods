using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;
using System.IO;

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
        public string ModuleTypeBasePadName => Util.camelCaseToLowercase(TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeBasePad>().GetType().Name);
        public ObjectList<UnityEngine.Mesh> GetBasePadMeshes()
        {
            return ModuleTypeBasePad.mMeshes;
        }
        public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            // Nothing required here
        }
    }
    public class ModuleTypeExteriorOxygenGenerator : ModuleType
	{
        public GameObject LoadOxygenGeneratorPrefab()
        {
            string text = "Prefabs/Modules/PrefabOxygenGenerator" + GetType().Name.Replace("ModuleType", string.Empty);
            GameObject original = ResourceUtil.loadPrefab(text);
            GameObject gameObject = Object.Instantiate(original);
            gameObject.calculateSmoothMeshRecursive(mMeshes);
            if (gameObject.GetComponent<Collider>() != null)
            {
                Debug.LogWarning(text + " has collision in the root");
            }
            GameObject gameObject2 = GameObject.Find(GroupName);
            if (gameObject2 == null)
            {
                gameObject2 = new();
                gameObject2.name = GroupName;
            }
            gameObject.transform.SetParent(gameObject2.transform, worldPositionStays: false);
            gameObject.SetActive(value: false);
            return gameObject;
        }
        public ModuleTypeExteriorOxygenGenerator()
		{
			string path = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\ExteriorO2Generator\\Textures\\ExteriorO2Generator.png";
            var thing = new ExteriorO2Genereator();
            if (File.Exists(path))
            {
                byte[] iconBytes = File.ReadAllBytes(path);
                Texture2D tex = new(0, 0);
                tex.LoadImage(iconBytes);
                this.mIcon = Util.applyColor(tex);
            }
            mPowerGeneration = -1000;
			mWaterGeneration = -1000;
			mMinSize = 0;
			mMaxSize = 0;
			mOxygenGeneration = 10;
			mFlags = 44560;
			mHeight = 0f;
            mBaseType = true;
            mExterior = true;
            mMeshes = thing.GetBasePadMeshes();
            mModels = new GameObject[Module.ValidSizes.Length];
            mRequiredStructure.set<ModuleTypeOxygenGenerator>();
			mCost = new ResourceAmounts();
            mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 2);
			mCost.add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), 2);
            initStrings();
        }
	}
}
