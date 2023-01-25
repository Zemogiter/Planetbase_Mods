using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Reflection;
using static Planetbase.Character;
using HarmonyLib;
using static Planetbase.Human;

namespace EternalBots
{
	public class EternalBots : ModBase
	{
        private AlertState m_activatedState;

        private bool m_autoActivated;

        public static new void Init(ModEntry modEntry) => InitializeMod(new EternalBots(), modEntry, "EternalBots");
       
		public override void OnInitialized(ModEntry modEntry)
		{
            m_activatedState = AlertState.NoAlert;
            m_autoActivated = false;
            TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().mFlags |= 32768;
            Debug.Log("[MOD] EternalBots activated");
        }
		
		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
			
		}
        public static void PrivateUpdate(float timeStep)
        {
            Character chara = Singleton<NewCharacter>.getInstance();
            if (chara.mTarget != null)
            {
                Selectable selectable = chara.mTarget.getSelectable();
                if (selectable != null && selectable.isDestroyed())
                {
                    chara.setIdle();
                }
            }
            if (chara.mQueuedAnimation != null)
            {
                var animationQueueTime = timeStep;

                if (animationQueueTime < 0f)
                {
                    chara.playAnimation(chara.mQueuedAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    if (chara.mQueuedAnchorPoint != null)
                    {
                        chara.setTransform(chara.mQueuedAnchorPoint.getPosition(), chara.mQueuedAnchorPoint.getRotation());
                    }
                    chara.mQueuedAnimation = null;
                }
            }
            if (chara.mState == State.Walking)
            {
                chara.updateWalking(timeStep);
            }
            else if (chara.mState == State.Interacting)
            {
                chara.updateInteracting(timeStep);
            }
            else if (chara.mState == State.Dead)
            {
                chara.updateDead(timeStep);
            }
            else if (chara.mState == State.Idle)
            {
                chara.updateIdle(timeStep);
            }
        }
    }
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch
    {
		public static bool Prefix(float timeStep)
		{
            Bot bot = Singleton<Bot>.getInstance();
            Character character = Singleton<NewCharacter>.getInstance();
            EternalBots.PrivateUpdate(timeStep);
            Indicator indicator = new(StringList.get("integrity"), ResourceList.StaticIcons.Bot, IndicatorType.Condition, 1f, 1f, SignType.Condition);
            indicator.setLevels(0.05f, 0.1f, 0.15f, 0.2f);
            character.mIndicators[7] = indicator;

            if (bot.shouldDecay())
            {
                character.decayIndicator(CharacterIndicator.Condition, timeStep / 480f);
            }
            Disaster stormInProgress = Singleton<DisasterManager>.getInstance().getStormInProgress();
            if (stormInProgress != null && !bot.isProtected())
            {
                character.decayIndicator(CharacterIndicator.Condition, timeStep * stormInProgress.getIntensity() / 600f);
            }
            SolarFlare solarFlare = Singleton<DisasterManager>.getInstance().getSolarFlare();
            if (solarFlare.isInProgress() && !bot.isProtected())
            {
                character.decayIndicator(CharacterIndicator.Condition, timeStep * solarFlare.getIntensity() / 180f);
            }
            bot.updateDustParticles(timeStep);

			return false;
        }
    }
    public class NewCharacter : Character
    {
        public override List<string> getAnimationNames(CharacterAnimationType animationType)
        {
            return mSpecialization.getAnimationNames(animationType, mLocation, Human.Gender.Unknown);
        }

        public override float getHeight()
        {
            return 1.25f;
        }

        public override Texture2D getIcon()
        {
            return mSpecialization.getIcon();
        }

        public override Bounds getSelectionBounds()
        {
            return new Bounds(getPosition() + Vector3.up * 0.6f, new Vector3(0.9f, 1.2f, 0.9f));
        }
    }
    public class NewBot : Bot
    {
        static NewBot()
        {
        }
    }
}
