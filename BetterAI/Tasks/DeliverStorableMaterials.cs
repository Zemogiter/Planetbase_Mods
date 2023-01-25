using Planetbase;

namespace BetterAI.Tasks
{
    class DeliverStorableMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && !loadedResource.isTraded())
            {
                ConstructionComponent storageComponent = Module.findStorageComponent(character, loadedResource.getResourceType());
                if (storageComponent != null)
                    if (AiRule.goTarget(character, (Selectable)storageComponent, (Selectable)null, Location.Unknown))
                    {
                        ai.CompleteTask();
                        return;
                    }

                Module storage = Module.findStorage(character);
                if (storage != null)
                {
                    StorageSlot storageSlot = storage.findStorageSlot(character.getPosition());
                    if (storageSlot != null)
                        if (AiRule.goTarget(character, new Target((Selectable)storage, storageSlot.getPosition()), (Selectable)null, Location.Unknown))
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