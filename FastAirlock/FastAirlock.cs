using Planetbase;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;

namespace FastAirlock
{
    public class FastAirlock : ModBase
    {
        public static float speedmult;
        public static new void Init(ModEntry modEntry) => InitializeMod(new FastAirlock(), modEntry, "FastAirlock");

        public override void OnInitialized(ModEntry modEntry)
        {
            var path = "./Mods/FastAirlock/config.txt";
            string line;
            System.IO.StreamReader file = new(path);
            line = file.ReadLine();
            line = line.Substring(13);
            speedmult = float.Parse(line);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing neede here
        }
    }
    [HarmonyPatch(typeof(InteractionAirlock), nameof(InteractionAirlock.update))]
    public class InteractionAirlockPatch
    {
        public static void Postfix(float timeStep, InteractionAirlock __instance)
        {
            timeStep *= FastAirlock.speedmult;
            Construction construction = __instance.mSelectable as Construction;
            if (construction != null && !construction.isPowered() && __instance.mStage == InteractionAirlock.Stage.Wait)
            {
                return;
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
                        return;
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
        }
    }
}