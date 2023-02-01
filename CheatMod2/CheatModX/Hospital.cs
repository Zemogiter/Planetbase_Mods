using Planetbase;

namespace CheatModX;

public class Hospital : ModuleType
{
	public Hospital()
	{
		GlobalVars instance = Singleton<GlobalVars>.getInstance();
		mCost = new ResourceAmounts(instance.oPANull);
		mCost.add(instance.oMetal, 10);
		mCost.add(instance.oBioplastic, 10);
		mPrestige = 100;
		mIcon = ResourceList.StaticIcons.Welfare;
		mPowerGeneration = -1000;
		mMinSize = 4;
		mMaxSize = 4;
		mComponentTypes = new ComponentType[2]
		{
			TypeList<ComponentType, ComponentTypeList>.find<SickBayBed>(),
			TypeList<ComponentType, ComponentTypeList>.find<MedicalCabinet>()
		};
		mFlags = 1072;
		mLayoutType = LayoutType.Normal;
		mRequiredStructure.set<ModuleTypeSickBay>();
		mName = "Hospital";
		mTooltip = "A much larger than regular Sickbay medication center, a facility to heal everyone in the colony.";
		mModels[4] = ResourceUtil.loadPrefab("Prefabs/Modules/PrefabStorage5");
	}
}
