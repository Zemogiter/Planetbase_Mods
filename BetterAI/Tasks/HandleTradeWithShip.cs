using Planetbase;

namespace BetterAI.Tasks
{
    class HandleTradeWithShip : AbstractTask
    {
        public override void Run(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && loadedResource.isTraded())
            {
                MerchantShip targetSelectable = character.getTargetSelectable() as MerchantShip;
                if (targetSelectable != null && targetSelectable.isTrading())
                {
                    character.unloadResource(Resource.State.Idle);
                    targetSelectable.tradeResource(loadedResource);
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