using System.Collections.Generic;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

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

        public static bool Prefix(Character character)
        {
            int maxTargeters = ((TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1));
            Module module = Module.findMine(character, true, maxTargeters);
            if (state == AlertState.YellowAlert && character.isMining())
            {
                return AiRule.goTarget(character, new Target(module));
            }
            return true;
        }
    }
}
