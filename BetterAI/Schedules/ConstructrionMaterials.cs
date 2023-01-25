using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    class ConstructionMaterials : AbstractSchedule
    {
        public ConstructionMaterials()
        {
            mType = SCHEDULE_TYPE.CONSTRUCTION_MATERIALS;

            mTaskList = new List<Tasks.AbstractTask>
            {
                // doesn't make sense here but normally used as fallback schedule
                //mTaskList.Add(new SetFallbackScheduleTypeTask(SCHEDULE_TYPE.IDLE));

                new Tasks.GetConstructionMaterials(),
                new Tasks.WaitForMovement(),
                new Tasks.LoadResource(),
                new Tasks.DeliverConstructionMaterials(),
                new Tasks.WaitForMovement(),
                new Tasks.DropConstructionMaterials()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}