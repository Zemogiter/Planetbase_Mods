using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using System.ComponentModel;
using static Planetbase.AiRule;

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
    public class AiRuleGoMinePatch  : AiRule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Could be written to at some point")]
        static AlertState state = SecurityManager.getInstance().getAlertState();
        static readonly List<Character> miners = Character.getSpecializationCharacters(SpecializationList.find("Driller"));
        static readonly List<Character> humanMiners = Character.getSpecializationCharacters(SpecializationList.find("Worker"));

        public static bool Prefix(Character character, AiRule __instance)
        {
            if (Singleton<ChallengeManager>.getInstance().isGameplayModifierActive(GameplayModifierType.DisableWorkerMining) && character is Human)
            {
                return false;
            }
            if (character.hasFlag(1) && __instance.canWork(character) && __instance.hasMinePriority(character) && Singleton<SecurityManager>.getInstance().isGoingOutsideAllowed())
            {
                //int maxTargeters = ((mPriority != Priority.EmptyOnly) ? (TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1) : 0);
                int maxTargeters = (TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1);
                Module module = Module.findMine(character, true, maxTargeters);
                if (module != null)
                {
                    return AiRule.goTarget(character, new Target(module));
                }
            }
            else if(character.hasFlag(1) && character.isProtected() && state == AlertState.YellowAlert)
            {
                int maxTargeters = (TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1);
                Module module = Module.findMine(character, true, maxTargeters);
                if (module != null)
                {
                    return AiRule.goTarget(character, new Target(module));
                }
            }

            return false;
        }

        public override bool update(Character character)
        {
            throw new System.NotImplementedException();
        }
    }
}
