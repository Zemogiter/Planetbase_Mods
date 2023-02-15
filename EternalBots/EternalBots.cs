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
    [HarmonyPatch(typeof(Bot), nameof(Bot.update))]
    public class BotPatch
    {
        public static bool Prefix(Bot __instance, float timeStep)
		{
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
    public sealed class MyCharacterImpl : Character
    {
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
        public static Indicator[] mIndicators = MyCharacterImpl.mIndicators;
        public static void decayIndicator(CharacterIndicator status, float amount)
        {
            var indicator = new MyCharacterImpl();
            indicator.decayIndicator(status, amount);
        }
    }
}
