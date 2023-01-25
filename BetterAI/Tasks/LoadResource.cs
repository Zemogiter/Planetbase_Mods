using System;
using Planetbase;

namespace BetterAI.Tasks
{
    class LoadResource : AbstractTask
    {
        public override void Run(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Resource targetResource = character.getTargetResource();
            if (targetResource == null ||
                targetResource.isDestroyed() ||
                targetResource.getState() != Resource.State.Idle && targetResource.getState() != Resource.State.Stored)
            {
                ai.FailTask();
                return;
            }

            if (targetResource.isEmbedded())
                targetResource.getContainer().extract(targetResource);

            character.loadResource(targetResource);
            character.setIdle((CharacterAnimation)null, CharacterAnimation.PlayMode.Immediate);

            ai.CompleteTask();
        }

        public override void Start(ScheduledState ai)
        {
        }
    }
}