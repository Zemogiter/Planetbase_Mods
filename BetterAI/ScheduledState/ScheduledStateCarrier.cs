using BetterAI.Schedules;
using BetterAI;

namespace BetterAI
{
    public abstract partial class ScheduledState : BaseState
    {
        private AbstractSchedule SelectCarrierSchedule()
        {
            if (HasCondition(CONDITION.MERCHANT_AWAITING_MATERIALS) &&
                HasCondition(CONDITION.MERCHANT_MATERIALS_AVAILABLE) &&
                HasCondition(CONDITION.FREE_TO_GO_OUTSIDE))
            {
                return GetScheduleOfType(SCHEDULE_TYPE.HANDLE_TRADE);
            }

            if (HasCondition(CONDITION.LOW_CONDITION))
            {
                return GetScheduleOfType(SCHEDULE_TYPE.SELF_REPAIR);
            }

            if (HasCondition(CONDITION.CONSTRUCTION_AWAITING_MATERIALS) &&
                HasCondition(CONDITION.CONSTRUCTION_MATERIALS_AVAILABLE))
            {
                return GetScheduleOfType(SCHEDULE_TYPE.CONSTRUCTION_MATERIALS);
            }

            if (HasCondition(CONDITION.TRANSFORMER_AWAITING_MATERIALS) &&
                HasCondition(CONDITION.TRANSFORMER_MATERIALS_AVAILABLE))
            {
                return GetScheduleOfType(SCHEDULE_TYPE.TRANFORMER_MATERIALS);
            }

            if (HasCondition(CONDITION.MATERIALS_AVAILABLE_FOR_STORAGECOMPONENT) ||
                HasCondition(CONDITION.MATERIALS_AVAILABLE_FOR_STORAGE))
            {
                return GetScheduleOfType(SCHEDULE_TYPE.HANDLE_STORAGE);
            }

            return GetScheduleOfType(SCHEDULE_TYPE.IDLE);
        }

    }
}