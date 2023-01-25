using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MinersIgnoreYellow
{
    public class MinersIgnoreYellow : ModBase
    {
        public Character character = Character.getFirstCharacter();
        public static new void Init(ModEntry modEntry) => InitializeMod(new MinersIgnoreYellow(), modEntry, "MinersIgnoreYellow");

        public override void OnInitialized(ModEntry modEntry)
        {
            Debug.Log("[MOD] Miners Ignore Yellow activated");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            var newAI = new CustomAI(AiRule.Priority.All);
            newAI.update(character);
           
        }
    }
    public class CustomAI : AiRuleGoMine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Could be written to at some point")]
        AlertState state = SecurityManager.getInstance().getAlertState();
        readonly List<Character> miners = Character.getSpecializationCharacters(SpecializationList.find("Driller"));
        readonly List<Character> humanMiners = Character.getSpecializationCharacters(SpecializationList.find("Worker"));

        public CustomAI(Priority priority) : base(priority)
        {
        }

        public override bool update(Character character)
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
                            return AiRule.goTarget(character, new Target(module));

                        }
                    }

                }
            }
            return false;
        }
    }
}
