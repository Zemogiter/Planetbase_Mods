using BetterAI;
using Planetbase;
using PlanetbaseModUtilities;

namespace BetterAI.Tasks
{
    class DropConstructionMaterials : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource loadedResource = character.getLoadedResource();
            if (loadedResource != null)
            {
                Buildable targetBuildable = character.getTargetBuildable();
                if (targetBuildable != null &&
                    targetBuildable.isAwaitingMaterials() &&
                    targetBuildable.getPredictedPendingConstructionCosts(character).containsResourceType(loadedResource.getResourceType()))
                {
                    targetBuildable.addConstructionMaterial(loadedResource);
                    character.unloadResource(Resource.State.Busy);

                    var specialization = CoreUtils.GetMember<Character, Specialization>("mSpecialization", character);
                    if (specialization is Constructor || specialization is Engineer)
                        ai.mNextScheduleType = SCHEDULE_TYPE.HANDLE_CONSTRUCTION;

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