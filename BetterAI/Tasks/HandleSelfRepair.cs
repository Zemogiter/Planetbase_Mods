using Planetbase;

namespace BetterAI.Tasks
{
    class HandleSelfRepair : AbstractTask
    {
        public override void Run(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            ConstructionComponent targetComponent = character.getTargetComponent();
            if (targetComponent != null && (double)targetComponent.getComponentType().getStatusRecoveryTime(CharacterIndicator.Condition) > 0.0)
            {
                if (targetComponent.isOperational())
                {
                    Interaction.create<InteractionComponentRestore>(character, (Selectable)targetComponent);
                    ai.CompleteTask();
                }
                return;
            }
            ai.FailTask();
        }

        public override void Start(ScheduledState ai)
        {
        }
    }
}