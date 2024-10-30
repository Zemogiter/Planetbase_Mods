using System.Collections.Generic;
using Planetbase;
using UnityEngine;

namespace BetterAI
{
    public abstract class OverrideAi : BaseAi
    {
        public static Dictionary<Character, CharacterState> mCharacterStates;

        public void UpdateIdle(Character character)
        {
#if DEBUG
            Debug.Log("updateIdle for " + character.getName());
#endif

            if (mCharacterStates == null)
                mCharacterStates = new Dictionary<Character, CharacterState>();

            if (!mCharacterStates.TryGetValue(character, out CharacterState cState))
            {
#if DEBUG
                Debug.Log("new cState");
#endif
                cState = new CharacterState()
                {
                    mCharacter = character
                };
                mCharacterStates.Add(character, cState);
            }

            this.mRuleTimer.start();
            if (character.isConscious() && cState != null)
                cState.RunAI();

            this.mRuleTimer.stop();
        }

        public void OnTargetReached(Character character)
        {
            if (!mCharacterStates.TryGetValue(character, out CharacterState cState))
                Debug.LogError("FATAL: CharacterState not found for: " + character.getName());

#if DEBUG
            Debug.Log("onTargetReached for: " + character.getName());
#endif
            cState.MovementComplete();
        }
    }
}