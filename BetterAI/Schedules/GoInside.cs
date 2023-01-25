using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    // basic survival schedule
    public class GoInside : AbstractSchedule
    {
        public GoInside()
        {
            mType = SCHEDULE_TYPE.GO_INSIDE;
            mTaskList = new List<Tasks.AbstractTask>
            {
                new Tasks.SetFallbackScheduleType(SCHEDULE_TYPE.IDLE),

                new Tasks.FindClosestAirlock(),
                new Tasks.StartWalking(),
                new Tasks.WaitForMovement(),
                new Tasks.Idle()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}