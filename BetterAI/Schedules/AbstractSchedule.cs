using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    public abstract class AbstractSchedule
    {
        public SCHEDULE_TYPE mType = SCHEDULE_TYPE.NONE;  // very important to always set this to a unique SCHEDULE_TYPE

        public List<Tasks.AbstractTask> mTaskList = null;

        public Dictionary<CONDITION, bool> mInterruptConditions = new Dictionary<CONDITION, bool>(); // a bit mask of conditions that can interrupt this schedule 
    }

    // default fallback and basic survival schedule
    public class Idle : AbstractSchedule
    {
        public Idle()
        {
            mType = SCHEDULE_TYPE.IDLE;

            mTaskList = new List<Tasks.AbstractTask>
            {
                // doesn't make sense here but normally used as fallback schedule
                //mTaskList.Add(new SetFallbackScheduleType(SCHEDULE_TYPE.IDLE));

                new Tasks.FindIdlePosition(),
                new Tasks.StartWalking(),
                new Tasks.WaitForMovement(),
                new Tasks.Idle()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}