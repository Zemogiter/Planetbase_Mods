using BetterAI;

namespace BetterAI.Tasks
{
    class WaitForMovement : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            if (ai.mMoveTarget == null && ai.mTaskStatus == TASK_STATE.ARRIVED)
                ai.CompleteTask();
        }

        public override void Run(ScheduledState ai)
        {
            if (ai.mMoveTarget == null && ai.mTaskStatus == TASK_STATE.ARRIVED)
                ai.CompleteTask();
        }
    }
}