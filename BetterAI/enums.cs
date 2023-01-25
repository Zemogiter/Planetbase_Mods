using System;

namespace BetterAI
{
    //=========================================================
    // State of the current task
    //=========================================================
    public enum TASK_STATE : long
    {
        NEW = 0,             // Just started
        RUNNING,             // Running task & movement
        ARRIVED,             // Movement completed
        COMPLETE,            // Completed, get next task
        FAILED,

        Count
    };

    //=========================================================
    // Each schedule must have a unique type
    //=========================================================
    public enum SCHEDULE_TYPE : long
    {
        NONE = 0,
        IDLE,
        GO_INSIDE,
        HANDLE_TRADE,
        SELF_REPAIR,
        CONSTRUCTION_MATERIALS,
        HANDLE_CONSTRUCTION,
        TRANFORMER_MATERIALS,
        HANDLE_TRANSFORMER,
        HANDLE_STORAGE,

        Count
    };

    //=========================================================
    // Conditions to use when sensing and selecting schedule
    //=========================================================
    public enum CONDITION : long
    {
        NONE = 0,
        SEE_ENEMY, // target entity is in full view.
        TAKEN_DAMAGE, // hurt 
        HAS_WEAPON,
        ENEMY_DEAD, // enemy was killed.

        IS_EXTERIOR, // exposed to vacuum
        IS_DETECTED,
        IS_AGGRESSIVE,
        IS_LOADED,
        IS_MINING,
        IS_FIT_FOR_WORK,
        IS_FIT_FOR_CRITICAL_WORK,
        IS_BEING_RESTORED,

        NO_BOT_CARRIER,
        NO_BOT_BUILDER,
        NO_BOT_MINER,
        NO_SPARES,
        MERCHANT_AWAITING_MATERIALS,
        MERCHANT_MATERIALS_AVAILABLE,
        CONSTRUCTION_AWAITING_MATERIALS,
        CONSTRUCTION_MATERIALS_AVAILABLE,
        TRANSFORMER_AWAITING_MATERIALS,
        TRANSFORMER_MATERIALS_AVAILABLE,
        MATERIALS_AVAILABLE_FOR_TRANSPORT,
        MATERIALS_AVAILABLE_FOR_STORAGE,
        MATERIALS_AVAILABLE_FOR_STORAGECOMPONENT,
        STORAGE_AVAILABLE_FOR_ITEMS,

        // security stuff
        FREE_TO_GO_OUTSIDE,
        YELLOW_ALERT,
        RED_ALERT,

        // character stats
        LOW_HEALTH,
        LOW_NUTRITION,
        LOW_HYDRATION,
        LOW_OXYGEN,
        LOW_SLEEP,
        LOW_MORALE,
        LOW_CONDITION,
        LOW_INTEGRITY,

        // critical conditions
        CRIT_HEALTH,
        CRIT_NUTRITION,
        CRIT_HYDRATION,
        CRIT_OXYGEN,
        CRIT_SLEEP,
        CRIT_MORALE,
        CRIT_CONDITION,
        CRIT_INTEGRITY,

        Count
    };

    /* awesome enum extensions for bitwise operations
       stolen from https://stackoverflow.com/a/417217 */
    public static class EnumerationExtensions
    {

        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((long)(object)type & (long)(object)value) == (long)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (long)(object)type == (long)(object)value;
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((long)(object)type | (long)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }


        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((long)(object)type & ~(long)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

    }
}