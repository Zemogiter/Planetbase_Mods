using Planetbase;

namespace BetterAI.Tasks
{
    class StartWalking : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            if (ai.mMoveTarget == null)
                ai.FailTask();

            Character character = ai.mCharacter;

            if (goTarget(character, ai.mMoveTarget))
            {
                character.resetWanderTime();
                ai.CompleteTask();
            }
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}