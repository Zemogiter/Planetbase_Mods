using System.Collections.Generic;
using BetterAI;
using BetterAI.Tasks;
using Planetbase;

namespace BetterAI.Tasks
{
    class StoreMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null && loadedResource.getLocation() == Location.Interior)
            {
                if (character.getTargetConstruction() is Module targetConstruction && targetConstruction.isOperational() && targetConstruction.hasFlag(ModuleType.FlagStorage))
                {
                    character.storeResource(targetConstruction);
                    character.setIdle((CharacterAnimation)null, CharacterAnimation.PlayMode.CrossFade);

                    ai.CompleteTask();
                    return;
                }

                ConstructionComponent targetComponent = character.getTargetComponent();
                if (targetComponent != null && targetComponent.isBuilt())
                {
                    List<ResourceType> neededResources = targetComponent.getNeededResources();
                    if (neededResources != null && neededResources.Contains(loadedResource.getResourceType()) && targetComponent.isSpaceAvailable())
                    {
                        character.embedResource(targetComponent, Resource.State.Busy);
                        character.setIdle((CharacterAnimation)null, CharacterAnimation.PlayMode.CrossFade);

                        ai.CompleteTask();
                        return;
                    }

                    List<ResourceType> storedResources = targetComponent.getComponentType().getStoredResources();
                    if (storedResources != null && storedResources.Contains(loadedResource.getResourceType()) && targetComponent.isSpaceAvailable())
                    {
                        character.embedResource(targetComponent, Resource.State.Stored);
                        character.setIdle((CharacterAnimation)null, CharacterAnimation.PlayMode.CrossFade);

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