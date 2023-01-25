using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    public class TransformerMaterials : AbstractSchedule
    {
        public TransformerMaterials()
        {
            mType = SCHEDULE_TYPE.TRANFORMER_MATERIALS;

            mTaskList = new List<Tasks.AbstractTask>
            {
                // doesn't make sense here but normally used as fallback schedule
                //mTaskList.Add(new SetFallbackScheduleType(SCHEDULE_TYPE.IDLE));

                new Tasks.FindTransformerMaterials(),
                new Tasks.WaitForMovement(),
                new Tasks.DeliverTransformerMaterials(),
                new Tasks.WaitForMovement(),
                new Tasks.FillTransformer()
            };

            mInterruptConditions.Add(CONDITION.TAKEN_DAMAGE, true);
        }
    }
}