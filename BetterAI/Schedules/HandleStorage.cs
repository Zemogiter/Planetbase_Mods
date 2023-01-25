using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    class HandleStorage : AbstractSchedule
    {
        public HandleStorage()
        {
            mType = SCHEDULE_TYPE.HANDLE_STORAGE;

            mTaskList = new List<Tasks.AbstractTask>
            {
                new Tasks.FindStorableMaterials(),
                new Tasks.WaitForMovement(),
                new Tasks.LoadResource(),
                new Tasks.DeliverStorableMaterials(),
                new Tasks.WaitForMovement(),
                new Tasks.StoreMaterials()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}