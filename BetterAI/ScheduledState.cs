using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterAI;
using BetterAI.Schedules;
using BetterAI.Tasks;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace BetterAI
{
    public abstract partial class ScheduledState : BaseState
    {
        public AbstractSchedule mSchedule = null;
        public bool mScheduleDone = false;
        public int mScheduleIndex = 0;
        public SCHEDULE_TYPE mFallbackScheduleType = SCHEDULE_TYPE.IDLE; // Schedule type to choose if current schedule fails
        public SCHEDULE_TYPE mNextScheduleType = SCHEDULE_TYPE.NONE; // Schedule type to chose if current schedule ends normally

        public TASK_STATE mTaskStatus = TASK_STATE.COMPLETE;

        private readonly Dictionary<SCHEDULE_TYPE, Type> mScheduleTypes;

        public ScheduledState()
        {
            /* stolen from http://www.jkfill.com/2010/12/29/self-registering-factories-in-c-sharp/ */
            mScheduleTypes = new Dictionary<SCHEDULE_TYPE, Type>();

            Assembly currAssembly = Assembly.GetExecutingAssembly();
            Type baseType = typeof(AbstractSchedule);

            foreach (Type type in currAssembly.GetTypes())
            {
                if (type == null || !type.IsClass || type.IsAbstract || !type.IsSubclassOf(baseType))
                    continue;

                if (System.Activator.CreateInstance(type) is AbstractSchedule scheduleType)
                {
                    SCHEDULE_TYPE newScheduleType = scheduleType.mType;
#if DEBUG
                    Debug.Log("Registering new schedule: " + type + " scheduleType " + newScheduleType);
#endif
                    mScheduleTypes.Add(newScheduleType, type);
                }
            }
        }

        //=========================================================
        // GetSchedule - Decides which type of schedule best suits
        // the ai's current state and conditions. Then calls
        // ai's member function to get a schedule of the proper type.
        //=========================================================
        public AbstractSchedule GetSchedule()
        {
            Character character = new NewCharacter();
            var specialization = CoreUtils.GetMember<Character, Specialization>("mSpecialization", character);

            if (specialization is Biologist)
                return SelectBiologistSchedule();

            else if (specialization is Carrier)
                return SelectCarrierSchedule();

            else if (specialization is Constructor)
                return SelectConstructorSchedule();

            else if (specialization is Driller)
                return SelectDrillerSchedule();

            else if (specialization is Engineer)
                return SelectEngineerSchedule();

            else if (specialization is Guard)
                return SelectGuardSchedule();

            else if (specialization is Intruder)
                return SelectIntruderSchedule();

            else if (specialization is Medic)
                return SelectMedicSchedule();

            else if (specialization is Visitor)
                return SelectVisitorSchedule();

            else if (specialization is Worker)
                return SelectWorkerSchedule();

            // todo load custom schedules here
            Debug.LogWarning("No schedule for specialization type, idling");

            return GetScheduleOfType(SCHEDULE_TYPE.IDLE);
        }

        //=========================================================
        // GetScheduleOfType - returns one of the 
        // ai's available schedules of the indicated type.
        //=========================================================
        public AbstractSchedule GetScheduleOfType(SCHEDULE_TYPE scheduleType)
        {
            if (!mScheduleTypes.TryGetValue(scheduleType, out Type newScheduleType))
            {
#if DEBUG
                Debug.LogWarning("Unknown Schedule Type " + scheduleType);
#endif
                return null;
            }

            return System.Activator.CreateInstance(newScheduleType) as AbstractSchedule;
        }

        //=========================================================
        // HasSchedule - Returns TRUE we have a schedule with tasks
        //=========================================================
        public bool HasSchedule()
        {
            return mSchedule != null && mSchedule.mTaskList != null;
        }

        //=========================================================
        // ClearSchedule - blanks out schedule and index.
        //=========================================================
        public void ClearSchedule()
        {
            mTaskStatus = TASK_STATE.NEW;
            mSchedule = null;
            mScheduleDone = false;
            mScheduleIndex = 0;
            mMoveTarget = null;
            mFallbackScheduleType = SCHEDULE_TYPE.IDLE;
            mNextScheduleType = SCHEDULE_TYPE.NONE;
        }

        //=========================================================
        // ChangeSchedule - clear states and set new schedule
        //=========================================================
        public void ChangeSchedule(AbstractSchedule newSchedule)
        {
#if DEBUG
            Debug.Log("ChangeSchedule to " + newSchedule);
#endif
            mSchedule = newSchedule;
            mScheduleDone = false;
            mScheduleIndex = 0;
            mFallbackScheduleType = SCHEDULE_TYPE.IDLE;
            mNextScheduleType = SCHEDULE_TYPE.NONE;

            // clear all the states
            mTaskStatus = TASK_STATE.NEW;
            mMoveTarget = null;

            ClearConditions();
        }

        //=========================================================
        // ScheduleInterruptConditions - strip off all bits excepts
        // the ones capable of breaking this schedule.
        //=========================================================
        public bool ScheduleInterruptConditions()
        {
            if (!HasSchedule())
                return false;

            Dictionary<CONDITION, bool> resultDict = mConditions.Where(x => mSchedule.mInterruptConditions.ContainsKey(x.Key))
                .ToDictionary(x => x.Key, x => mSchedule.mInterruptConditions[x.Key]);

            return resultDict.Count > 0;
        }

        //=========================================================
        // ScheduleIsValid - returns TRUE as long as the current
        // schedule is still the proper schedule to be executing,
        // taking into account all conditions
        //=========================================================
        public bool ScheduleIsValid()
        {
            if (!HasSchedule() ||
                TaskHasFailed() ||
                mScheduleDone ||
                ScheduleInterruptConditions())
            {
                return false;
            }

            return true;
        }

        //=========================================================
        // MaintainSchedule - does all the per-think schedule maintenance.
        // ensures that the ai leaves this function with a valid
        // schedule!
        //=========================================================
        public void MaintainSchedule()
        {
            AbstractSchedule newSchedule;
            int i, maxI = 10;
#if DEBUG
            Debug.Log("maintaining schedule");
#endif

            if (HasSchedule())
                maxI = mSchedule.mTaskList.Count;

            // try to finish as many tasks as there are,
            // or fall back to 10 retries for a new schedule
            for (i = 0; i < maxI; i++)
            {
                if (HasSchedule() && TaskIsComplete())
                    NextScheduledTask();

                // validate existing schedule 
                if (!ScheduleIsValid())
                {
#if DEBUG
                    Debug.Log("!ScheduleIsValid");
#endif
                    if (TaskHasFailed())
                    {
                        if (mFallbackScheduleType != SCHEDULE_TYPE.NONE)
                        {
                            newSchedule = GetScheduleOfType(mFallbackScheduleType);
                            mFallbackScheduleType = SCHEDULE_TYPE.NONE;
                        }
                        else
                            newSchedule = GetScheduleOfType(SCHEDULE_TYPE.IDLE);
#if DEBUG
                        Debug.Log("Task " + (mScheduleIndex + 1) + " failed from schedule " + mSchedule);
#endif
                    }
                    else if (mNextScheduleType != SCHEDULE_TYPE.NONE)
                    {
                        newSchedule = GetScheduleOfType(mNextScheduleType);
                        mNextScheduleType = SCHEDULE_TYPE.NONE;
                    }
                    else
                        newSchedule = GetSchedule();

                    ChangeSchedule(newSchedule);
                }
#if DEBUG
                else
                    Debug.Log("ScheduleIsValid");
#endif

                if (TaskIsNew())
                {
                    AbstractTask pTask = GetTask();

                    if (pTask != null)
                    {
                        BeginTask();
                        pTask.Start(this);
                    }
                    else
                        Debug.LogWarning("no task to start");
                }

                if (!TaskIsComplete() && !TaskIsNew())
                    break;
            }

            if (!TaskIsComplete() && !TaskHasFailed())
            {
                AbstractTask pTask = GetTask();
                if (pTask != null)
                    pTask.Run(this);
                else
                    Debug.LogWarning("no task to continue on");
            }
        }

        //=========================================================
        // NextScheduledTask - increments the ScheduleIndex
        //=========================================================
        private void NextScheduledTask()
        {
            mTaskStatus = TASK_STATE.NEW;
            mScheduleIndex++;
#if DEBUG
            Debug.Log("NextScheduledTask " + mScheduleIndex);
#endif
            if (HasSchedule() && mScheduleIndex >= mSchedule.mTaskList.Count)
            {
#if DEBUG
                Debug.Log("ScheduleIsDone");
#endif
                mScheduleDone = true;
            }
        }

        //=========================================================
        // GetTask - returns the current scheduled task. NULL if there's a problem.
        //=========================================================
        private AbstractTask GetTask()
        {
            if (!HasSchedule() || mScheduleIndex < 0 || mScheduleIndex >= mSchedule.mTaskList.Count)
            {
                // mScheduleIndex is not within valid range for the ai's current schedule.
                return null;
            }

            return mSchedule.mTaskList[mScheduleIndex];
        }

        public void CompleteTask()
        {
            if (!TaskHasFailed())
            {
#if DEBUG
                Debug.Log("TaskIsComplete");
#endif
                mTaskStatus = TASK_STATE.COMPLETE;
            }
        }

        public void FailTask()
        {
            if (!TaskIsComplete())
            {

#if DEBUG
                Debug.Log("TaskHasFailed");
#endif
                mTaskStatus = TASK_STATE.FAILED;
            }
        }

        public void BeginTask()
        {
            if (TaskIsNew())
            {
#if DEBUG
                Debug.Log("TaskHasStarted");
#endif
                mTaskStatus = TASK_STATE.RUNNING;
            }
        }

        public bool TaskIsNew()
        {
            return mTaskStatus == TASK_STATE.NEW;
        }

        public bool TaskIsRunning()
        {
            return mTaskStatus == TASK_STATE.RUNNING;
        }

        public bool TaskIsComplete()
        {
            return mTaskStatus == TASK_STATE.COMPLETE;
        }

        public bool TaskHasFailed()
        {
            return mTaskStatus == TASK_STATE.FAILED;
        }

        //=========================================================
        // MovementComplete - the movement task was finished (or we are in an airlock)
        //=========================================================
        public void MovementComplete()
        {
            // handle airlocks exclusively
            Construction targetConstruction = mCharacter.getTargetConstruction();
            if (targetConstruction != null &&
                targetConstruction.hasFlag(ModuleType.FlagAirlock) &&
                targetConstruction.isOperational() &&
                targetConstruction.getInteractionCount() <= 5)
            {
                if (mCharacter.getLocation() == Location.Interior)
                    Interaction.create<InteractionAirlockExit>(mCharacter, (Selectable)targetConstruction);
                else
                    Interaction.create<InteractionAirlockEnter>(mCharacter, (Selectable)targetConstruction);

#if DEBUG
                Debug.Log("handled airlock, correct?");
#endif

                // try to go back in schedule to repeat the last target task
                mScheduleIndex -= 2;
#if DEBUG
                Debug.Log("going one back in schedule " + mScheduleIndex);
#endif
                mTaskStatus = TASK_STATE.COMPLETE;
                return;
            }

            mMoveTarget = null;

            if (!TaskIsRunning())
                Debug.LogWarning("MovementComplete without Task?");
            else
            {
                /* we need to set the character idle here, to get
                  the game to call the thinking method for us again */
                mCharacter.setIdle((CharacterAnimation)null, CharacterAnimation.PlayMode.CrossFade);
                mTaskStatus = TASK_STATE.ARRIVED;
            }
        }
    }
    public class NewCharacter : Character
    {
        public override float getHeight()
        {
            throw new NotImplementedException();
        }

        public override Texture2D getIcon()
        {
            throw new NotImplementedException();
        }

        public override Bounds getSelectionBounds()
        {
            throw new NotImplementedException();
        }

        public override List<string> getAnimationNames(CharacterAnimationType animationType)
        {
            throw new NotImplementedException();
        }
    }
}