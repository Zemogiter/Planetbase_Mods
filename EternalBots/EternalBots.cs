using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Linq;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using System.Collections.Generic;

namespace EternalBots
{
    public class EternalBots : ModBase
	{
        public new static void Init(ModEntry modEntry) => InitializeMod(new EternalBots(), modEntry, "EternalBots");
       
		public override void OnInitialized(ModEntry modEntry)
		{
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
                // decrement then test the new value
                animationTime -= timeStep;
                CoreUtils.SetMember<Character, float>("mAnimationQueueTime", __instance, animationTime);
                if (animationTime <= 0f)
                {
                    var queuedAnimation = CoreUtils.GetMember<Character, CharacterAnimation>("mQueuedAnimation", __instance);
                    if (queuedAnimation != null)
                    {
                        __instance.playAnimation(queuedAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    }
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
            // Run our custom update and skip the original to avoid duplicate updates.
            CustomBot.PrivateUpdate(__instance, timeStep);

            Indicator indicator = new Indicator(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Condition, 1f, 1f, SignType.Condition);
            indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);

            // Replace index 7 safely: handle arrays and IList, avoid out-of-range.
            var indicatorsEnum = __instance.getIndicators();
            if (indicatorsEnum != null)
            {
                // If it's an IList<Indicator> we can set directly
                if (indicatorsEnum is IList<Indicator> ilist)
                {
                    if (ilist.Count > 7) ilist[7] = indicator;
                }
                // If it's an array
                else if (indicatorsEnum is Indicator[] arr)
                {
                    if (arr.Length > 7) arr[7] = indicator;
                }
                // Otherwise try a safe fallback: convert to list and try to assign back if possible
                else
                {
                    try
                    {
                        var tmp = indicatorsEnum.ToList();
                        if (tmp.Count > 7)
                        {
                            tmp[7] = indicator;
                            // There's no generic setter known for indicators here; best effort only.
                        }
                    }
                    catch
                    {
                        // ignore — avoid throwing during update
                    }
                }
            }

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
            if (solarFlare != null && solarFlare.isInProgress() && !__instance.isProtected())
            {
                __instance.decayIndicator(CharacterIndicator.Condition, timeStep * solarFlare.getIntensity() / 180f);
            }
            CoreUtils.InvokeMethod<Bot>("updateDustParticles", __instance, timeStep);

            // We handled the update — prevent the original Bot.update from also running.
            return false;
        }
    }
}
