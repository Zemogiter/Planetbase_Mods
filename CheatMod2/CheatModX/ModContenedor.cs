using System.Collections.Generic;
using Planetbase;
using UnityEngine;

namespace CheatModX;

public class ModContenedor : ComponentType
{
	public ModContenedor()
	{
		GlobalVars instance = Singleton<GlobalVars>.getInstance();
		mConstructionCosts = new ResourceAmounts(instance.oPANull);
		mConstructionCosts.add(instance.oBioplastic, 1);
		mEmbeddedResourceCount = instance.iMaxContainer;
		mStoredResources = new List<ResourceType>();
		foreach (ResourceType item in TypeList<ResourceType, ResourceTypeList>.get())
		{
			if (item.mModel != null)
			{
				mStoredResources.Add(item);
			}
		}
		mIcon = ResourceList.StaticIcons.Morale;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		gameObject.transform.localScale = new Vector3(1f, 0.125f, 1f);
		GameObject gameObject2 = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Modules/PrefabOxygenGenerator1")).findNamedObject("oxygen_generator_1");
		gameObject2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		gameObject2.transform.parent = gameObject.transform;
		mModel = gameObject;
		mName = "Colony Controls";
		mTooltip = "An advanced device to edit almost every aspect in the game, also can be used as a huge storage facility.";
	}
}
