using Planetbase;

namespace BetterAI.Tasks
{
    class FillTransformer : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && !loadedResource.isTraded())
            {
                ConstructionComponent targetComponent = character.getTargetComponent();
                if (targetComponent != null &&
                    (targetComponent.getNeededResources() != null &&
                    targetComponent.getNeededResources().Contains(loadedResource.getResourceType())) &&
                    targetComponent.isSpaceAvailable())
                {
                    character.embedResource(targetComponent, Resource.State.Busy);
                    ai.CompleteTask();
                    return;
                }
            }
            ai.FailTask();
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}