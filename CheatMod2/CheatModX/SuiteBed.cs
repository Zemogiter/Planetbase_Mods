using Planetbase;
using UnityEngine;

namespace CheatModX;

public class SuiteBed : ComponentType
{
	public SuiteBed()
	{
		mConstructionCosts = new ResourceAmounts();
		mConstructionCosts.add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), 1);
		mIcon = ResourceUtil.loadIcon("Components/icon_bed");
		mStatusRecoveryTimes = new float[8];
		mStatusRecoveryTimes[4] = 200f;
		mStatusRecoveryTimes[5] = 400f;
		mPrimaryStatusRecovery = CharacterIndicator.Sleep;
		addUsageAnimation(CharacterAnimationType.LieDown);
		mTransitionInAnimations = new CharacterAnimation[1]
		{
			new CharacterAnimation(CharacterAnimationType.LieDownIn)
		};
		mTransitionOutAnimations = new CharacterAnimation[1]
		{
			new CharacterAnimation(CharacterAnimationType.LieDownOut)
		};
		mFlags = 33554434;
		mRadius = 1.25f;
		mName = "SuiteBed";
		mTooltip = "A better bed to refresh colonist.";
		mModel = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Components/PrefabSickBayBed"));
	}
}
