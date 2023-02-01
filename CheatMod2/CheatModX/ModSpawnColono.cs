using Planetbase;
using UnityEngine;

namespace CheatModX;

public class ModSpawnColono : ComponentType
{
	public ModSpawnColono()
	{
		GlobalVars instance = Singleton<GlobalVars>.getInstance();
		mConstructionCosts = new ResourceAmounts(instance.oPANull);
		mConstructionCosts.add(instance.oBioplastic, 1);
		mIcon = ResourceList.StaticIcons.Visitor;
		GameObject gameObject = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Components/PrefabBotAutoRepair").findNamedObject("bot_auto_repair"));
		gameObject.AddComponent<Light>();
		mModel = gameObject;
		mName = StringList.get("colonists") + " Caller";
		mTooltip = "An advanced device to call any number of any colonist, including bots.";
	}
}
