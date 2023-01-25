using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    public class HandleConstruction : AbstractSchedule
    {
        public HandleConstruction()
        {
            mType = SCHEDULE_TYPE.HANDLE_CONSTRUCTION;

            mTaskList = new List<Tasks.AbstractTask>
            {
                new Tasks.FindConstructionToBuild(),
                new Tasks.WaitForMovement(),
                new Tasks.Build()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}