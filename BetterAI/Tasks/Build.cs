using Planetbase;

namespace BetterAI.Tasks
{
    class Build : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Buildable targetBuildable = character.getTargetBuildable();
            if (targetBuildable != null && targetBuildable.getBuildLocation() == character.getLocation() && targetBuildable.isAwaitingBuilder())
            {
                Interaction.create<InteractionBuild>(character, (Selectable)targetBuildable);
                return;
            }
        }

        public override void Run(ScheduledState ai)
        {
            ai.CompleteTask();
        }
    }
}