using System.Collections.Generic;
using Planetbase;

namespace BetterAI.Tasks
{
    class FindTransformerMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            TransformerList transformers = ConstructionComponent.findTransformers(character);
            for (int i = 0; i < transformers.getCount(); ++i)
            {
                TransformerEntry transformer = transformers.getTransformer(i);
                if (Singleton<ManufactureLimits>.getInstance().isUnderLimit(transformer.getTransformer()))
                {
                    List<ResourceType> predictedNeededResources = transformer.getPredictedNeededResources();
                    int countResourceIndex = Resource.getHighestCountResourceIndex(predictedNeededResources);
                    for (int index = 0; index < predictedNeededResources.Count; ++index)
                    {
                        ResourceType resourceType = predictedNeededResources[(index + countResourceIndex) % predictedNeededResources.Count];
                        if (AiRule.goGetResource(character, transformer.getTransformer().getPosition(), Location.Interior, resourceType, (Selectable)transformer.getTransformer(), false))
                        {
                            ai.CompleteTask();
                            return;
                        }
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