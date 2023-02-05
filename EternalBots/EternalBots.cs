using Planetbase;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using State = Planetbase.Character.State;
using System.Collections.Generic;

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
    /*[HarmonyPatch(typeof(Character), nameof(Character.update))]
    public class CharacterPatch
    {
        public static bool Prefix(Character __instance)
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
                var animationQueueTime = timeStep;

                if (animationQueueTime < 0f)
                {
                    __instance.playAnimation(__instance.mQueuedAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    if (__instance.mQueuedAnchorPoint != null)
                    {
                        __instance.setTransform(__instance.mQueuedAnchorPoint.getPosition(), __instance.mQueuedAnchorPoint.getRotation());
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
            return false;
        }
    }*/
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch
    {
		public static void Postfix(float timeStep, Bot __instance)
		{
            Indicator indicator = new(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Condition, 1f, 1f, SignType.Condition);
            indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
            MyCharacterImpl.mIndicators[7] = indicator;

            if (__instance.shouldDecay())
            {
                MyCharacter.decayIndicator(CharacterIndicator.Condition, timeStep / 480f);
            }
            Disaster stormInProgress = Singleton<DisasterManager>.getInstance().getStormInProgress();
            if (stormInProgress != null && !__instance.isProtected())
            {
                MyCharacter.decayIndicator(CharacterIndicator.Condition, timeStep * stormInProgress.getIntensity() / 600f);
            }
            SolarFlare solarFlare = Singleton<DisasterManager>.getInstance().getSolarFlare();
            if (solarFlare.isInProgress() && !__instance.isProtected())
            {
                MyCharacter.decayIndicator(CharacterIndicator.Condition, timeStep * solarFlare.getIntensity() / 180f);
            }
            __instance.updateDustParticles(timeStep);

        }
    }
    public sealed class MyCharacterImpl : Character
    {
        //To-do: add WORKING implementation for mIndicators[]
        public static new Indicator[] mIndicators = new Indicator[8];
        public new bool decayIndicator(CharacterIndicator status, float amount)
        {
            Indicator indicator = mIndicators[(int)status];
            bool result = false;
            if (indicator != null)
            {
                result = indicator.isMin();
                if (indicator != null && !indicator.isMin())
                {
                    result = indicator.decrease(amount);
                    updateSign(indicator, indicator.getSignType());
                }
            }
            return result;
        }
        //methods below are here just to stop compiler errors
        public override List<string> getAnimationNames(CharacterAnimationType animationType)
        {
            throw new System.NotImplementedException();
        }

        public override float getHeight()
        {
            throw new System.NotImplementedException();
        }

        public override Texture2D getIcon()
        {
            throw new System.NotImplementedException();
        }

        public override Bounds getSelectionBounds()
        {
            throw new System.NotImplementedException();
        }
    }
    public class MyCharacter
    {
        public static void decayIndicator(CharacterIndicator status, float amount)
        {
            var indicator = new MyCharacterImpl();
            indicator.decayIndicator(status, amount);
        }
    }
}
