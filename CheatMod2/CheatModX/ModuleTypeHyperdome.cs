using Planetbase;
using UnityEngine;

namespace CheatModX;

public class ModuleTypeHyperdome : ModuleType
{
	public ModuleTypeHyperdome()
	{
		GlobalVars instance = Singleton<GlobalVars>.getInstance();
		mCost = new ResourceAmounts(instance.oPANull);
		mCost.add(instance.oMetal, 1);
		mCost.add(instance.oBioplastic, 1);
		mComponentTypes = new ComponentType[15]
		{
			TypeList<ComponentType, ComponentTypeList>.find<ModSpawnColono>(),
			TypeList<ComponentType, ComponentTypeList>.find<ModContenedor>(),
			TypeList<ComponentType, ComponentTypeList>.find<MealMaker>(),
			TypeList<ComponentType, ComponentTypeList>.find<DrinksMachine>(),
			TypeList<ComponentType, ComponentTypeList>.find<Table>(),
			TypeList<ComponentType, ComponentTypeList>.find<TableSmall>(),
			TypeList<ComponentType, ComponentTypeList>.find<BarTable>(),
			TypeList<ComponentType, ComponentTypeList>.find<BarTableNoChairs>(),
			TypeList<ComponentType, ComponentTypeList>.find<DrinkingFountain>(),
			TypeList<ComponentType, ComponentTypeList>.find<DecorativePlant>(),
			TypeList<ComponentType, ComponentTypeList>.find<ExerciseBar>(),
			TypeList<ComponentType, ComponentTypeList>.find<Treadmill>(),
			TypeList<ComponentType, ComponentTypeList>.find<Bench>(),
			TypeList<ComponentType, ComponentTypeList>.find<PineTree>(),
			TypeList<ComponentType, ComponentTypeList>.find<OakTree>()
		};
		mIcon = ResourceList.StaticIcons.Day;
		mName = "Hyperdome";
		mTooltip = "Central Module containing advanced devices to edit the game.";
		mFlags = 1058864;
		mMinSize = 4;
		mMaxSize = 4;
		mOxygenGeneration = 100;
		mWaterGeneration = 1000;
		mLayoutType = LayoutType.Cross;
		mModels[4] = Resources.Load<GameObject>("Prefabs/Modules/PrefabBioDome5");
	}
}
