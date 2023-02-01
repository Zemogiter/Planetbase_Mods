using Planetbase;
using UnityEngine;

namespace CheatModX;

public class SuperDorm : ModuleType
{
	public SuperDorm()
	{
		GlobalVars instance = Singleton<GlobalVars>.getInstance();
		mCost = new ResourceAmounts(instance.oPANull);
		mCost.add(instance.oMetal, 10);
		mCost.add(instance.oBioplastic, 10);
		mPrestige = 100;
		mIcon = ResourceList.StaticIcons.SwitchPlanet;
		mPowerGeneration = -1000;
		mMinSize = 4;
		mMaxSize = 4;
		mComponentTypes = new ComponentType[2]
		{
			TypeList<ComponentType, ComponentTypeList>.find<Bunk>(),
			TypeList<ComponentType, ComponentTypeList>.find<Bed>()
		};
		mFlags = 33808;
		mLayoutType = LayoutType.Normal;
		mRequiredStructure.set<ModuleTypeCabin>();
		mName = "Superdorm";
		mTooltip = "A supersized sleeping facility for colonist and visitors.";
		GameObject gameObject = Resources.Load<GameObject>("Prefabs/Modules/PrefabStorage5");
		mModels[4] = gameObject;
		mLayoutType = LayoutType.Normal;
	}
}
