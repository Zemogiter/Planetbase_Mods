using Planetbase;

namespace BetterAI.Tasks
{
    class GoGetWeapon : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            AiRule.goGetResource(character, character.getPosition(), character.getLocation(), TypeList<ResourceType, ResourceTypeList>.find<Gun>(), (Selectable)null, false);
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}