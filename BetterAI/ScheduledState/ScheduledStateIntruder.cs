using BetterAI;
using BetterAI.Schedules;

namespace BetterAI
{
    public abstract partial class ScheduledState : BaseState
    {
        private AbstractSchedule SelectIntruderSchedule()
        {
            return GetScheduleOfType(SCHEDULE_TYPE.IDLE);
        }
    }
}