using Planetbase;
using UnityEngine;

namespace CheatModX;

public class ShipMonument : ModuleType
{
	public GameObject kapal;

	public ShipMonument()
	{
		kapal = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Ships/PrefabShipPersonnelBigColonist"));
		mName = "ShipMonument";
		mTooltip = "An old space ship transformed to storage house after it stops operating.";
		mIcon = ResourceUtil.loadIcon("Ships/icon_ship_personnel_big");
		mPowerGeneration = -1000;
		mMinSize = 4;
		mMaxSize = 4;
		mFlags = 56;
		Object.Instantiate(Resources.Load<GameObject>("Prefabs/Modules/PrefabBioDome5")).findNamedObject("bio_dome_5_floor").transform.parent = kapal.transform;
		Object.Instantiate(Resources.Load<GameObject>("Prefabs/Modules/PrefabBioDome5")).findNamedObject("bio_dome_5_base").transform.parent = kapal.transform;
		ResourceList.getInstance().PrefabBlinkingLight.transform.parent = kapal.transform;
		kapal.transform.localPosition = new Vector3(0f, 2.5f, 0f);
		kapal.playDefaultAnimation(100f);
		mModels[4] = kapal;
		mLayoutType = LayoutType.Circular;
		mRequiredStructure.set<ModuleTypeOxygenGenerator>();
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		kapal.GetComponent<Rigidbody>().AddTorque(5f, 0f, 0f);
		kapal.transform.Rotate(new Vector3(0f, 15f, 0f) * Time.deltaTime, Space.Self);
	}
}
