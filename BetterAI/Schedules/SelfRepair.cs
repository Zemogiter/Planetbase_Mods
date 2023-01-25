using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    public class SelfRepair : AbstractSchedule
    {
        public SelfRepair()
        {
            mType = SCHEDULE_TYPE.SELF_REPAIR;

            mTaskList = new List<Tasks.AbstractTask>
            {
                // doesn't make sense here but normally used as fallback schedule
                //mTaskList.Add(new SetFallbackScheduleTypeTask(SCHEDULE_TYPE.IDLE));

                new Tasks.FindFreeRepairStation(),
                new Tasks.StartWalking(),
                new Tasks.WaitForMovement(),
                new Tasks.HandleSelfRepair()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}