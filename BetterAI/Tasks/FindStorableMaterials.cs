using BetterAI;
using BetterAI.Tasks;
using Planetbase;
using PlanetbaseModUtilities;

namespace BetterAi.Tasks
{
    class FindStorableMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource storable;

            foreach (ResourceType resourceType in )
            {
                ConstructionComponent storageComponent = Module.findStorageComponent(character, resourceType);
                if (storageComponent != null)
                {
                    storable = Resource.findStorable(character, resourceType, true);
                    if (storable != null)
                        if (AiRule.goTarget(character, (Selectable)storable, (Selectable)storageComponent, Location.Unknown))
                        {
                            ai.CompleteTask();
                            return;
                        }
                }
            }

            Module storage = Module.findStorage(character);
            if (storage != null)
            {
                storable = Resource.findStorable(character, (ResourceType)null, false);
                if (storable != null)
                    if (AiRule.goTarget(character, (Selectable)storable, (Selectable)storage, Location.Unknown))
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