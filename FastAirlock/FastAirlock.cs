using Planetbase;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine.UI;

namespace FastAirlock
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Speed multiplier for airlock animations")] public float speedmult = 3;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class FastAirlock : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new FastAirlock(), modEntry, "FastAirlock");
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            //nothing required here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing neede here
        }
    }
    [HarmonyPatch(typeof(InteractionAirlock), nameof(InteractionAirlock.update))]
    public class InteractionAirlockPatch
    {
        public static bool Prefix(InteractionAirlock __instance, ref bool __result, float timeStep)
        {
            __result = ReplacementMethod(__instance, timeStep);
            return false;
        }
        public static bool ReplacementMethod(InteractionAirlock __instance, float timeStep)
        {
            timeStep *= FastAirlock.settings.speedmult;
            Construction construction = __instance.mSelectable as Construction;
            if (construction != null && !construction.isPowered() && __instance.mStage == InteractionAirlock.Stage.Wait)
            {
                return true;
            }
            if (__instance.mSelectable.getFirstInteraction() == __instance)
            {
                __instance.mStageProgress += timeStep;
                if (__instance.mStageProgress > 1f || __instance.mStage == InteractionAirlock.Stage.Wait)
                {
                    bool flag = __instance.mStage == InteractionAirlock.Stage.Exit;
                    __instance.onStageDone();
                    __instance.mStageProgress = 0f;
                    if (flag)
                    {
                        return true;
                    }
                }
            }
            else
            {
                __instance.mStage = InteractionAirlock.Stage.Wait;
                __instance.mTarget = __instance.getQueuePosition(__instance.mSelectable.getInteractionIndex(__instance));
            }
            Vector3 direction = __instance.mTarget - __instance.mCharacter.getPosition();
            float magnitude = direction.magnitude;
            float d = Mathf.Min(4f * timeStep, magnitude);
            if (magnitude > 0.25f)
            {
                Vector3 target;
                if (__instance.mStage == InteractionAirlock.Stage.Wait && magnitude < 1f)
                {
                    target = (__instance.mSelectable.getPosition() - __instance.mCharacter.getPosition()).flatDirection();
                }
                else
                {
                    target = direction.flatDirection();
                }
                Vector3 direction2 = __instance.mCharacter.getDirection();
                __instance.mCharacter.setPosition(__instance.mCharacter.getPosition() + direction.normalized * d);
                __instance.mCharacter.setDirection(Vector3.RotateTowards(direction2, target, 6.28318548f * timeStep, 0.1f));
                if (__instance.mAnimationType != CharacterAnimationType.Walk)
                {
                    __instance.mAnimationType = CharacterAnimationType.Walk;
                    __instance.mCharacter.playWalkAnimation();
                }
            }
            else
            {
                if (__instance.mStage == InteractionAirlock.Stage.GoEntry)
                {
                    __instance.mStageProgress = 1f;
                }
                if (__instance.mAnimationType != CharacterAnimationType.Idle)
                {
                    __instance.mAnimationType = CharacterAnimationType.Idle;
                    __instance.mCharacter.playIdleAnimation(CharacterAnimation.PlayMode.CrossFade);
                }
            }
            return false;
        }
    }
}