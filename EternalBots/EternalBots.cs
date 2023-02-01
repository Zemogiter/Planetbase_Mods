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
    }
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch
    {
		public static bool Prefix(float timeStep)
		{
            Bot bot;
            Character character;
            if (character.mTarget != null)
            {
                Selectable selectable = character.mTarget.getSelectable();
                if (selectable != null && selectable.isDestroyed())
                {
                    character.setIdle();
                }
            }
            if (character.mQueuedAnimation != null)
            {
                var animationQueueTime = timeStep;

                if (animationQueueTime < 0f)
                {
                    character.playAnimation(character.mQueuedAnimation, WrapMode.Loop, CharacterAnimation.PlayMode.Immediate);
                    if (character.mQueuedAnchorPoint != null)
                    {
                        character.setTransform(character.mQueuedAnchorPoint.getPosition(), character.mQueuedAnchorPoint.getRotation());
                    }
                    character.mQueuedAnimation = null;
                }
            }
            if (character.mState == State.Walking)
            {
                character.updateWalking(timeStep);
            }
            else if (character.mState == State.Interacting)
            {
                character.updateInteracting(timeStep);
            }
            else if (character.mState == State.Dead)
            {
                character.updateDead(timeStep);
            }
            else if (character.mState == State.Idle)
            {
                character.updateIdle(timeStep);
            }
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
}
