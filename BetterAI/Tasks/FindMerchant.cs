using Planetbase;

namespace BetterAI.Tasks
{
    class FindMerchantAndStartWalking : AbstractTask
    {
        public override void Run(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && loadedResource.isTraded())
            {
                MerchantShip merchantShip = Ship.find(loadedResource.getTraderId()) as MerchantShip;
                if (merchantShip != null)
                    if (AiRule.goTarget(character, new Target((Selectable)merchantShip, merchantShip.getPosition() - merchantShip.getDirection() * merchantShip.getRadius()), (Selectable)null, Location.Unknown))
                    {
                        ai.CompleteTask();
                        return;
                    }
            }
            ai.FailTask();
        }

        public override void Start(ScheduledState ai)
        {
        }
    }
}