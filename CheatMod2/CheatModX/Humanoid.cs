using Planetbase;
using UnityEngine;

namespace CheatModX;

public class Humanoid : Specialization
{
	public Humanoid()
	{
		addHeads(CharacterDefinitions.getInstance().Medic);
		mAi = HumanoidAi.getInstance();
		mCharacterType = typeof(Colonist);
		mColor = CharacterDefinitions.getInstance().Medic.MainColor;
		mDefaultRatio = 0f;
		mFlags = 135;
		mIcon = ResourceList.StaticIcons.Male;
		mIconFemale = ResourceList.StaticIcons.Female;
		mIntegrityFactor = 10f;
		GameObject gameObject = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Characters/PrefabMaleTracksuit"));
		gameObject.AddComponent<Light>();
		mModel = gameObject;
		GameObject gameObject2 = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Characters/PrefabFemaleTracksuit"));
		gameObject2.AddComponent<Light>();
		mModelFemale = gameObject2;
		GameObject gameObject3 = Object.Instantiate(ResourceUtil.loadPrefab("Prefabs/Characters/PrefabAstronaut"));
		gameObject3.AddComponent<Light>();
		mModelExterior = gameObject3;
		mModelTracksuit = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabMaleTracksuit");
		mModelTracksuitFemale = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabFemaleTracksuit");
		mName = GetType().Name;
		mNameFemale = GetType().Name;
		mNamePlural = GetType().Name;
		mNamePrefix = "AND";
		mSpeedFactor = 2f;
		mValue = 500;
	}
}
