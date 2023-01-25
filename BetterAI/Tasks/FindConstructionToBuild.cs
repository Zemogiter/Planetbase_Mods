using Planetbase;

namespace BetterAI.Tasks
{
    class FindConstructionToBuild : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Buildable buildable = (Buildable)Construction.findAwaitingBuilder(character) ?? (Buildable)ConstructionComponent.findAwaitingBuilder(character);
            if (buildable != null)
            {
                Target target = new Target((Selectable)buildable);
                target.setRadius(buildable.getRadius() + 0.5f);
                if (AiRule.goTarget(character, target, (Selectable)null, buildable.getBuildLocation()))
                {
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
    