using Planetbase;

namespace BetterAI.Tasks
{
    class Idle : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            ai.mCharacter.setIdle((CharacterAnimation)null, CharacterAnimation.PlayMode.CrossFade);
            ai.CompleteTask();
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}