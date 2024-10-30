using BetterAI;
using BetterAI.Schedules;

namespace BetterAI
{
    public abstract partial class ScheduledState : BaseState
    {
        private AbstractSchedule SelectEngineerSchedule()
        {
            return GetScheduleOfType(SCHEDULE_TYPE.IDLE);
        }
    }
}