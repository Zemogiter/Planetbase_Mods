using Planetbase;

namespace BetterAI.Tasks
{
    class FindClosestAirlock : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            if (character.getLocation() != Location.Exterior)
                ai.CompleteTask();

            Module closestAirlock = Module.findClosestAirlock(character, character.getPosition());
            if (closestAirlock == null)
                return;

            if (!character.isWaitingForAirlock(closestAirlock))
            {
                ai.mMoveTarget = AiRule.getAirlockTarget(closestAirlock, character);
                ai.CompleteTask();
            }
        }

        public override void Run(ScheduledState ai)
        {
            Start(ai);
        }
    }
}