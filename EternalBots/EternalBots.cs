using Planetbase;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using State = Planetbase.Character.State;
using System.Collections.Generic;
using System;

namespace EternalBots
{
	public class EternalBots : ModBase
	{
        public static new void Init(ModEntry modEntry) => InitializeMod(new EternalBots(), modEntry, "EternalBots");
       
		public override void OnInitialized(ModEntry modEntry)
		{
            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().mFlags |= 32768;
            Debug.Log("[MOD] EternalBots activated");
        }
		
		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
			
		}
    }
    public class CustomBot : Bot
    {
        public static void PrivateUpdate(Bot __instance, float timeStep)
        {
            if (__instance.mTarget != null)
            {
                Selectable selectable = __instance.mTarget.getSelectable();
                if (selectable != null && selectable.isDestroyed())
                {
                    __instance.setIdle();
                }
            }
            if (__instance.mQueuedAnimation != null)
            {
                __instance.mAnimationQueueTime -= timeStep;
                if (__instance.mAnimationQueueTime < 0f)
                {
                    __instance.playAnimation(__instance.mQueuedAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    if (__instance.mQueuedAnchorPoint != null)
                    {
                        __instance.setTransform(__instance.mQueuedAnchorPoint.getPosition(), __instance.mQueuedAnchorPoint.getRotation());
                        __instance.mQueuedAnchorPoint = null;
                    }
                    __instance.mQueuedAnimation = null;
                }
            }
            if (__instance.mState == State.Walking)
            {
                __instance.updateWalking(timeStep);
            }
            else if (__instance.mState == State.Interacting)
            {
                __instance.updateInteracting(timeStep);
            }
            else if (__instance.mState == State.Dead)
            {
                __instance.updateDead(timeStep);
            }
            else if (__instance.mState == State.Idle)
            {
                __instance.updateIdle(timeStep);
            }
        }
    }
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch : Bot
    {
        public static bool Prefix(Bot __instance, float timeStep)
		{
            CustomBot.PrivateUpdate(__instance, timeStep);

            //Indicator indicator = new(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Condition, 1f, 1f, SignType.Condition);
            //indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
            //MyCharacter.mIndicators[7] = indicator;

            Indicator indicator = new(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Normal, 1f, 1f, SignType.Condition);
            indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
            indicator.setOrientation(IndicatorOrientation.Vertical);
            CharacterIndicator IntegrityIndicator = CharacterIndicator.Integrity;
            __instance.getIndicator(IntegrityIndicator).setValue(indicator.getMax());

            if (__instance.shouldDecay())
            {
                __instance.decayIndicator(CharacterIndicator.Condition, timeStep / 480f);
            }
            Disaster stormInProgress = Singleton<DisasterManager>.getInstance().getStormInProgress();
            if (stormInProgress != null && !__instance.isProtected())
            {
                __instance.decayIndicator(CharacterIndicator.Condition, timeStep * stormInProgress.getIntensity() / 600f);
            }
            SolarFlare solarFlare = Singleton<DisasterManager>.getInstance().getSolarFlare();
            if (solarFlare.isInProgress() && !__instance.isProtected())
            {
                __instance.decayIndicator(CharacterIndicator.Condition, timeStep * solarFlare.getIntensity() / 180f);
            }
            __instance.updateDustParticles(timeStep);

            return false;
        }
    }
}
