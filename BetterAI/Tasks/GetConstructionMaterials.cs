using System.Collections.Generic;
using Planetbase;
using UnityEngine;

namespace BetterAI.Tasks
{
    class GetConstructionMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            IList<Buildable> awaitingMaterials = Buildable.findAwaitingMaterials(character);
            for (int index1 = 0; index1 < awaitingMaterials.Count; ++index1)
            {
                Buildable buildable = awaitingMaterials[index1];
                ResourceAmounts constructionCosts = buildable.getPredictedPendingConstructionCosts(character);
                if (constructionCosts != null && !constructionCosts.isEmpty())
                {
                    int index2 = Random.Range(0, constructionCosts.getCount());
                    ResourceType resourceType = constructionCosts.get(index2).getResourceType();
                    if (AiRule.goGetResource(character, buildable.getPosition(), buildable.getBuildLocation(), resourceType, (Selectable)buildable, false))
                    {
                        ai.CompleteTask();
                        return;
                    }
                }
            }
            ai.FailTask();
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}