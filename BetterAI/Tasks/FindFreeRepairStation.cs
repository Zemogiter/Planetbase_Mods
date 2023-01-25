using Planetbase;

namespace BetterAI.Tasks
{
    class FindFreeRepairStation : AbstractTask
    {
        public override void Run(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            ConstructionComponent free = ConstructionComponent.findFree(character, 4, false);
            if (free != null)
            {
                if (AiRule.goTarget(character, (Selectable)free, (Selectable)null, Location.Unknown))
                    ai.CompleteTask();
            }
            else
                ai.FailTask();
        }

        public override void Start(ScheduledState ai)
        {
        }
    }
}