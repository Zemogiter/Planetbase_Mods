using Planetbase;
using UnityEngine;

namespace CheatModX;

public class Suite : ModuleType
{
	public Suite()
	{
		mIcon = ResourceUtil.loadIcon("Modules/icon_cabin");
		mPowerGeneration = -500;
		mMinSize = 1;
		mMaxSize = 1;
		mComponentTypes = new ComponentType[7]
		{
			TypeList<ComponentType, ComponentTypeList>.find<SuiteBed>(),
			TypeList<ComponentType, ComponentTypeList>.find<TableSmall>(),
			TypeList<ComponentType, ComponentTypeList>.find<Table>(),
			TypeList<ComponentType, ComponentTypeList>.find<DrinkingFountain>(),
			TypeList<ComponentType, ComponentTypeList>.find<MealMaker>(),
			TypeList<ComponentType, ComponentTypeList>.find<DecorativePlant>(),
			TypeList<ComponentType, ComponentTypeList>.find<VideoScreen>()
		};
		mLayoutType = LayoutType.Cross;
		mFlags = 33808;
		mRequiredStructure.set<ModuleTypeCabin>();
		mName = "Suite";
		mTooltip = "A Personal bedroom for special people.";
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Modules/PrefabLab2"));
		mModels[1] = gameObject;
	}
}
