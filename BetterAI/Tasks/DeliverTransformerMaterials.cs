using Planetbase;
using UnityEngine;

namespace BetterAI.Tasks
{
    class DeliverTransformerMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && !loadedResource.isTraded())
            {
                ConstructionComponent transformer = ConstructionComponent.findTransformer(character, loadedResource.getResourceType(), (ComponentType)null);
                if (transformer != null)
                {
                    Transform resourcePoint = transformer.getResourcePoint(0);
                    if ((Object)resourcePoint != (Object)null)
                        if (AiRule.goTarget(character, new Target((Selectable)transformer, resourcePoint.position, resourcePoint.rotation), (Selectable)null, Location.Unknown))
                        {
                            ai.CompleteTask();
                            return;
                        }

                    if (AiRule.goTarget(character, (Selectable)transformer, (Selectable)null, Location.Unknown))
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