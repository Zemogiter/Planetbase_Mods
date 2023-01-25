using Planetbase;

namespace BetterAI.Tasks
{
    class FindTradedResourceAndStartWalking : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource traded = Resource.findTraded(character);
            if (traded != null)
            {
                Ship ship = Ship.find(traded.getTraderId());
                if (ship != null)
                {
                    int num = 3 + Module.getCategoryModules(Module.Category.Airlock).Count * 2;
                    if (ship.getTargeterCount() < num)
                        if (AiRule.goTarget(character, (Selectable)traded, (Selectable)ship, Location.Unknown))
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