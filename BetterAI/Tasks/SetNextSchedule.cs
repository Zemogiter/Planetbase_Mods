using BetterAI;

namespace BetterAI.Tasks
{
    public class SetNextScheduleType : AbstractTask
    {
        private SCHEDULE_TYPE mNextSchedule;

        public SetNextScheduleType(SCHEDULE_TYPE nextSchedule)
        {
            mNextSchedule = nextSchedule;
        }

        public override void Start(ScheduledState ai)
        {
            ai.mNextScheduleType = mNextSchedule;
            ai.CompleteTask();
        }

        public override void Run(ScheduledState ai)
        {
        }
    }
}