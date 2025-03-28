using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace MinersIgnoreYellow
{
    public class MinersIgnoreYellow : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new MinersIgnoreYellow(), modEntry, "MinersIgnoreYellow");

        public override void OnInitialized(ModEntry modEntry)
        {
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            /*var characters = CharacterUtils.GetAllCharacters();
            var stayInMine = AiRuleStayInMine;
            foreach (Character character in characters)
            {
                if(CoreUtils.InvokeMethod<Character,bool>("isMining", character, null) && SecurityManager.getInstance().getAlertState() == AlertState.YellowAlert)
                {
                    character.setCurrentAiRule(stayInMine);
                }
            }*/
        }
    }
    public class AiRuleStayInMine : AiTargetRule //to-do: figure out how to add this to DrillerAi and HumanAi
    {
        public override bool update(Character character)
        {
            if (character.isProtected() && SecurityManager.getInstance().getAlertState() == AlertState.YellowAlert)
            {
                Construction targetConstruction = character.getTargetConstruction();
                if (targetConstruction != null && targetConstruction.isOperational() && targetConstruction.hasFlag(2) && targetConstruction.getInteractionCount() < targetConstruction.getMaxUsers())
                {
                    Interaction.create<InteractionWork>(character, targetConstruction);
                    return true;
                }
            }
            return false;
        }
    }
    /*
    [HarmonyPatch(typeof(AiRuleGoMine), nameof(AiRuleGoMine.update))]
    public class AiRuleGoMinePatch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Could be written to at some point")]
        static AlertState state = SecurityManager.getInstance().getAlertState();

        public static bool Prefix(Character character)
        {
            int maxTargeters = ((TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1));
            Module module = Module.findMine(character, true, maxTargeters);
            if (SecurityManager.getInstance().getAlertState() == AlertState.YellowAlert)
            {
                //return AiRule.goTarget(character, new Target(module));
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.onAlert))]
    public class AlertPatch
    {
        public static void Postfix(Character __instance)
        {
            int maxTargeters = ((TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeMine>().getMaxUsers() - 1));
            Module module = Module.findMine(__instance, true, maxTargeters);
            if (SecurityManager.getInstance().getAlertState() == AlertState.YellowAlert)
            {
                if(__instance.getSpecialization() == SpecializationList.getColonistSpecializations().Find(x => x.getName() == "Worker") || __instance.getSpecialization() == SpecializationList.getColonistSpecializations().Find(x => x.getName() == "Driller"))
                {
                    //var mineRule = new AiRuleGoMine(AiRule.Priority.HighPriorityOnly);
                    //__instance.setCurrentAiRule(mineRule);
                }
            }
        }
    }
    */
}
