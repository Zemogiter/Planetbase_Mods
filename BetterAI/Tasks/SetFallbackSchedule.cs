using BetterAI;

namespace BetterAI.Tasks
{
    public class SetFallbackScheduleType : AbstractTask
    {
        private SCHEDULE_TYPE mFallbackSchedule;

        public SetFallbackScheduleType(SCHEDULE_TYPE fallbackSchedule)
        {
            mFallbackSchedule = fallbackSchedule;
        }

        public override void Start(ScheduledState ai)
        {
            ai.mFallbackScheduleType = mFallbackSchedule;
            ai.CompleteTask();
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}