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
            //Console.WriteLine("The value of speedmult is " + speedmult);
        }
    }
    [HarmonyPatch(typeof(InteractionAirlock), nameof(InteractionAirlock.update))]
    public class InteractionAirlockPatch
    {
        public static bool Prefix(float timeStep)
        {
            timeStep *= FastAirlock.speedmult;
            var airlock = NewInteractionAirlock.getInstance();
            if (GameManager.getInstance().getGameState() is GameStateGame game)
            {
                airlock.mSelectable = airlock.getSelectable();
                if (airlock.mSelectable is Construction construction && !construction.isPowered() && airlock.mStage == InteractionAirlock.Stage.Wait)
                {
                    return true;
                }
                //Console.WriteLine("this.mSelectable is= " + airlock.mSelectable);

                if (airlock.mTarget != null && airlock.mSelectable is Construction && airlock.mSelectable.getFirstInteraction() == airlock)
                {
                    airlock.mStageProgress += timeStep;
                    //Console.WriteLine("this.mTarget is= " + airlock.mTarget);
                    //Console.WriteLine("this.mSelectable is= " + airlock.mSelectable);
                    //Console.WriteLine("this.mStageProgress is= " + airlock.mStageProgress);
                    if (airlock.mStageProgress > 1f || airlock.mStage == InteractionAirlock.Stage.Wait)
                    {
                        //Console.WriteLine("this.mStage is= " + airlock.mStage);
                        bool flag = airlock.mStage == InteractionAirlock.Stage.Exit;
                        Console.WriteLine("flag is= " + flag);
                        airlock.onStageDone();
                        airlock.mStageProgress = 0f;
                        if (flag)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    airlock.mStage = InteractionAirlock.Stage.Wait;
                    airlock.mTarget = airlock.getQueuePosition(airlock.mSelectable.getInteractionIndex(airlock));
                }

                Vector3 direction = airlock.mTarget - airlock.mCharacter.getPosition();
                //Console.WriteLine("direction is= " + direction);
                float magnitude = direction.magnitude;
                //Console.WriteLine("magnitude is= " + magnitude);
                float d = Mathf.Min(4f * timeStep, magnitude);
                //Console.WriteLine("d is= " + d);
                if (magnitude > 0.25f && direction != null)
                {
                    Vector3 target;
                    if (airlock.mStage == InteractionAirlock.Stage.Wait && magnitude < 1f && airlock.mSelectable is Construction && direction != null)
                    {
                        target = (airlock.mSelectable.getPosition() - airlock.mCharacter.getPosition()).flatDirection();
                    }
                    else
                    {
                        target = direction.flatDirection();
                    }
                    Vector3 direction2 = airlock.mCharacter.getDirection();
                    //Console.WriteLine("direction2 is= " + direction2);
                    airlock.mCharacter.setPosition(airlock.mCharacter.getPosition() + direction.normalized * d);
                    airlock.mCharacter.setDirection(Vector3.RotateTowards(direction2, target, 6.28318548f * timeStep, 0.1f));
                    if (airlock.mAnimationType != CharacterAnimationType.Walk)
                    {
                        airlock.mAnimationType = CharacterAnimationType.Walk;
                        airlock.mCharacter.playWalkAnimation();
                    }
                }
                else
                {
                    if (airlock.mStage == InteractionAirlock.Stage.GoEntry)
                    {
                        airlock.mStageProgress = 1f;
                    }
                    if (airlock.mAnimationType != CharacterAnimationType.Idle)
                    {
                        airlock.mAnimationType = CharacterAnimationType.Idle;
                        airlock.mCharacter.playIdleAnimation(CharacterAnimation.PlayMode.CrossFade);
                    }   
                }
            }
            return false;
        }
    }
    public class NewInteractionAirlock : InteractionAirlock
    {
        public override Vector3 getQueuePosition(int i)
        {
            throw new NotImplementedException();
        }

        public override void onStageDone()
        {
            throw new NotImplementedException();
        }
        public static InteractionAirlock mInstance;
        public static InteractionAirlock getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new NewInteractionAirlock();
            }
            return mInstance;
        }
    }
}