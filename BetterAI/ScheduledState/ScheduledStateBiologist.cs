using BetterAI;
using BetterAI.Schedules;

namespace BetterAI
{
    public abstract partial class ScheduledState : BaseState
    {
        private AbstractSchedule SelectBiologistSchedule()
        {
            return GetScheduleOfType(SCHEDULE_TYPE.IDLE);
        }
    }
}