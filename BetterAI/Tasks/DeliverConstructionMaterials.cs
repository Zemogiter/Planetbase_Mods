using Planetbase;

namespace BetterAI.Tasks
{
    class DeliverConstructionMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && !loadedResource.isTraded())
            {
                Buildable awaitingMaterials = Buildable.findAwaitingMaterials(character, loadedResource.getResourceType());
                if (awaitingMaterials != null)
                {
                    Target target = new Target((Selectable)awaitingMaterials, awaitingMaterials.getPosition() + MathUtil.randFlatVector(awaitingMaterials.getRadius() * 0.5f));
                    target.setRadius(awaitingMaterials.getRadius() * 0.5f);
                    if (AiRule.goTarget(character, target, (Selectable)null, awaitingMaterials.getBuildLocation()))
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