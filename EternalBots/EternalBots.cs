using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace EternalBots
{
    public class EternalBots : ModBase
	{
        public new static void Init(ModEntry modEntry) => InitializeMod(new EternalBots(), modEntry, "EternalBots");
       
		public override void OnInitialized(ModEntry modEntry)
		{
            //TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().mFlags |= 32768;
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
            if (__instance.getTarget() != null)
            {
                Selectable selectable = __instance.getTarget().getSelectable();
                if (selectable != null && selectable.isDestroyed())
                {
                    __instance.setIdle();
                }
            }
            if (__instance.getUsageAnimations() != null)
            {
                var animationTime = CoreUtils.GetMember<Character, float>("mAnimationQueueTime", __instance);
                CoreUtils.SetMember<Character, float>("mAnimationQueueTime", __instance, animationTime - timeStep);
                if (animationTime < 0f)
                {
                    var queuedAnimation = CoreUtils.GetMember<Character, CharacterAnimation>("mQueuedAnimation", __instance);
                    __instance.playAnimation(queuedAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    if (__instance.getAnchorPoint() != null)
                    {
                        CoreUtils.SetMember<Character, SimpleTransform>("mQueuedAnchorPoint", __instance, null);
                    }
                    CoreUtils.SetMember<Character, CharacterAnimation>("mQueuedAnimation", __instance, null);
                }
            }
            if (__instance.getState() == State.Walking)
            {
                CoreUtils.InvokeMethod<Character>("updateWalking", __instance, timeStep);
            }
            else if (__instance.getState() == State.Interacting)
            {
                CoreUtils.InvokeMethod<Character>("updateInteracting", __instance, timeStep);
            }
            else if (__instance.getState() == State.Dead)
            {
                CoreUtils.InvokeMethod<Character>("updateDead", __instance, timeStep);
            }
            else if (__instance.getState() == State.Idle)
            {
                CoreUtils.InvokeMethod<Character>("updateIdle", __instance, timeStep);
            }
        }
    }
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch
    {
        public static bool Prefix(Bot __instance, float timeStep)
		{
            //PrivateUpdate
            CustomBot.PrivateUpdate(__instance, timeStep);

            Indicator indicator = new Indicator(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Condition, 1f, 1f, SignType.Condition);
            indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
            var targetIndicator = __instance.getIndicators().ToList();
            targetIndicator[7] = indicator;

            if (CoreUtils.InvokeMethod<Bot, bool>("shouldDecay", __instance))
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
            CoreUtils.InvokeMethod<Bot>("updateDustParticles", __instance, timeStep);

            return true;
        }
    }
}
