using System.Collections.Generic;
using Planetbase;
using UnityEngine;

namespace BetterAI
{
    public abstract class BaseState
    {
        public Character mCharacter = null;
        public Character mEnemy = null;

        public Target mMoveTarget = null;

        //=========================================================
        // Conditions to use when sensing and selecting schedule
        //=========================================================
        protected Dictionary<CONDITION, bool> mConditions = new Dictionary<CONDITION, bool>();

        public void ClearConditions()
        {
#if DEBUG
            Debug.Log("ClearConditions");
#endif
            mConditions.Clear();
        }

        public void AddCondition(CONDITION cond, bool state = true)
        {
#if DEBUG
            Debug.Log("AddCondition " + cond + " " + state);
#endif
            mConditions.Add(cond, state);
        }

        public void RemoveCondition(CONDITION cond)
        {
#if DEBUG
            Debug.Log("RemoveCondition " + cond);
#endif
            mConditions.Remove(cond);
        }

        public bool HasCondition(CONDITION cond)
        {
            if (mConditions.TryGetValue(cond, out bool state))
                return state;

            return false;
        }
    }
}