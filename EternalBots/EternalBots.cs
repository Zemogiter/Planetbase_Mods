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
                Type typecontroller = typeof(Bot);
                FieldInfo finfo = typecontroller.GetField("mAnimationQueueTime", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                float animationTime = (float)finfo.GetValue(finfo);
                finfo.SetValue(finfo, animationTime -  timeStep);
                if (animationTime < 0f)
                {
                    FieldInfo quotedAnimation = typecontroller.GetField("mQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                    CharacterAnimation characterAnimation = (CharacterAnimation)quotedAnimation.GetValue(quotedAnimation);
                    __instance.playAnimation(characterAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    if (__instance.getAnchorPoint() != null)
                    {
                        __instance.setTransform(__instance.getAnchorPoint().position, __instance.getAnchorPoint().rotation);
                        FieldInfo queuedAnchorPoint = typecontroller.GetField("mQueuedAnchorPoint", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                        queuedAnchorPoint.SetValue(queuedAnchorPoint, null);
                    }
                    quotedAnimation.SetValue(quotedAnimation, null);
                }
            }
            if (__instance.getState() == State.Walking)
            {
                CoreUtils.InvokeMethod<Character>("updateWalking", __instance, timeStep);
                //__instance.updateWalking(timeStep);
                MethodInfo dynMethod = __instance.GetType().GetMethod("updateWalking", BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(__instance, [timeStep]);
            }
            else if (__instance.getState() == State.Interacting)
            {
                CoreUtils.InvokeMethod<Character>("updateInteracting", __instance, timeStep);
                //__instance.updateInteracting(timeStep);
                MethodInfo dynMethod = __instance.GetType().GetMethod("updateInteracting", BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(__instance, [timeStep]);
            }
            else if (__instance.getState() == State.Dead)
            {
                CoreUtils.InvokeMethod<Character>("updateDead", __instance, timeStep);
                //__instance.updateDead(timeStep);
                MethodInfo dynMethod = __instance.GetType().GetMethod("updateDead", BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(__instance, [timeStep]);
            }
            else if (__instance.getState() == State.Idle)
            {
                CoreUtils.InvokeMethod<Character>("updateIdle", __instance, timeStep);
                //__instance.updateIdle(timeStep);
                MethodInfo dynMethod = __instance.GetType().GetMethod("updateIdle", BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(__instance, [timeStep]);
            }
        }
    }
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch
    {
        public static bool Prefix(Bot __instance, float timeStep)
		{
            CustomBot.PrivateUpdate(__instance, timeStep);

            var indicators = __instance.getIndicators().ToList();
            Indicator indicator = indicators.Find(i => i.getName() == "Integrity");
            indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
            indicator.setOrientation(IndicatorOrientation.Vertical);
            indicator.setValue(indicator.getMax());

            Type type = typeof(Bot);
            var boolInstance = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod("shouldDecay", BindingFlags.NonPublic | BindingFlags.Instance);
            bool result = (bool)method.Invoke(boolInstance, null);

            if (result)
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
            //__instance.updateDustParticles(timeStep);
            MethodInfo dynMethod = __instance.GetType().GetMethod("updateDustParticles", BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod.Invoke(__instance, [timeStep]);

            return false;
        }
    }
}
