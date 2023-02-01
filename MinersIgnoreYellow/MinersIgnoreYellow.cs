using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using System.ComponentModel;

namespace MinersIgnoreYellow
{
    public class MinersIgnoreYellow : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new MinersIgnoreYellow(), modEntry, "MinersIgnoreYellow");

        public override void OnInitialized(ModEntry modEntry)
        {
            Debug.Log("[MOD] Miners Ignore Yellow activated");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
           
        }
    }
    [HarmonyPatch(typeof(AiRuleGoMine), nameof(AiRuleGoMine.update))]
    public class AiRuleGoMinePatch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Could be written to at some point")]
        static AlertState state = SecurityManager.getInstance().getAlertState();
        static readonly List<Character> miners = Character.getSpecializationCharacters(SpecializationList.find("Driller"));
        static readonly List<Character> humanMiners = Character.getSpecializationCharacters(SpecializationList.find("Worker"));

        public static bool Prefix(Character character, AiRuleGoMine __instance)
        {
            int maxTargeters = (TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1);
            Module module = Module.findMine(character, true, maxTargeters);

            if (character.getState() != 0)
            {
                return false;
            }

            //main code, first we check if disasters are in progress (miners probably shouldn't ignore intruder attacks)
            if (DisasterManager.getInstance().anyInProgress())
            {
                //not sure how to combine both miner groups so I'm using two foreach, probably a bad idea
                foreach (Bot driller in miners.Cast<Bot>())
                {
                    foreach (Character worker in humanMiners)
                    {
                        //keeps miners in the mine if yellow alert is active
                        if (state == AlertState.YellowAlert && driller.isProtected() || worker.isProtected())
                        {
                            object[] parameters = { character, new Target(module) };
                            bool target = CoreUtils.InvokeMethod<AiRule, bool>("goTarget", __instance, parameters);
                            return target;
                        }
                    }

                }
            }
            return true;
        }
    }
}
