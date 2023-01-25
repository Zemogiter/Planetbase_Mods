using System.Collections.Generic;
using UnityEngine.UI;

namespace BetterAI.Schedules
{
    // basic survival schedule
    public class HandleTrade : AbstractSchedule
    {
        public HandleTrade()
        {
            mType = SCHEDULE_TYPE.HANDLE_TRADE;
            mTaskList = new List<Tasks.AbstractTask>
            {
                new Tasks.FindTradedResourceAndStartWalking(),
                new Tasks.WaitForMovement(),
                new Tasks.LoadResource(),
                new Tasks.FindMerchantAndStartWalking(),
                new Tasks.WaitForMovement(),
                new Tasks.HandleTradeWithShip()
            };

            mInterruptConditions.Add(CONDITION.RED_ALERT, true);
            mInterruptConditions.Add(CONDITION.YELLOW_ALERT, true);
        }
    }
}