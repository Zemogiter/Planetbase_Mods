using System;
using System.Collections.Generic;
using Planetbase;

namespace CheatModX;

public class HumanoidAi : BaseAi
{
	[NonSerialized]
	public static HumanoidAi mHumanoidAi;

	public HumanoidAi()
	{
		mIdleRules = new List<AiRule>();
		mTargetRules = new List<AiTargetRule>();
		mIdleRules.Add(new AiRuleGoGetWeapon());
		mIdleRules.Add(new AiRuleGoAttackIntruder());
		mIdleRules.Add(new AiRuleFleeIntruder());
		mIdleRules.Add(new AiRuleReleaseWeapon());
		mIdleRules.Add(new AiRuleGoEmergencyRepair());
		addHumanSurvivalRules();
		mIdleRules.Add(new AiRuleGoEmergencyFoodMaintenance());
		mIdleRules.Add(new AiRuleGoEmergencyFillSparesWorkshops());
		mIdleRules.Add(new AiRuleGoLoadResourcesToTrade());
		mIdleRules.Add(new AiRuleGoTradeLoadedResources());
		mIdleRules.Add(new AiRuleGoBuild());
		mIdleRules.Add(new AiRuleGoRelax(IndicatorLevel.VeryLow));
		mIdleRules.Add(new AiRuleGoGetResourceToRepair());
		mIdleRules.Add(new AiRuleGoRepair());
		mIdleRules.Add(new AiRuleGoGetResourceToRestore());
		mIdleRules.Add(new AiRuleGoRestore());
		mIdleRules.Add(new AiRuleGoGetConstructionMaterials());
		mIdleRules.Add(new AiRuleGoDeliverConstructionMaterials());
		mIdleRules.Add(new AiRuleGoLoadResourcesForTransformers());
		mIdleRules.Add(new AiRuleGoFillTransformers());
		mIdleRules.Add(new AiRuleGoGetResourceToConsume<AlcoholicDrink>(CharacterIndicator.Morale, IndicatorLevel.Suboptimal));
		mIdleRules.Add(new AiRuleGoRelax(IndicatorLevel.Suboptimal));
		mIdleRules.Add(new AiRuleGoLoadResourcesToStore());
		mIdleRules.Add(new AiRuleGoStoreLoadedResources());
		mIdleRules.Add(new AiRuleDropResource());
		mIdleRules.Add(new AiRuleGoInterior());
		mIdleRules.Add(new AiRuleGetOutOfTheWay());
		mIdleRules.Add(new AiRuleWanderInterior());
		mTargetRules.Add(new AiRulePickUpWeapon());
		mTargetRules.Add(new AiRuleCombat());
		mTargetRules.Add(new AiRuleRepair());
		addHumanSurvivalTargetRules();
		mTargetRules.Add(new AiRuleTrade());
		mTargetRules.Add(new AiRuleRestore());
		mTargetRules.Add(new AiRuleStoreResource());
		mTargetRules.Add(new AiRuleEmbedResource());
		mTargetRules.Add(new AiRuleDropConstructionMaterials());
		mTargetRules.Add(new AiRuleBuild());
		mTargetRules.Add(new AiRuleRelax());
		mTargetRules.Add(new AiRuleSetIdle());
	}

	public static HumanoidAi getInstance()
	{
		if (mHumanoidAi == null)
		{
			mHumanoidAi = new HumanoidAi();
		}
		return mHumanoidAi;
	}
}
