using Planetbase;
using UnityEngine;

namespace BetterAI.Tasks
{
    public abstract class AbstractTask : AiRule
    {
        public abstract void Start(ScheduledState ai);
        public abstract void Run(ScheduledState ai);

        public override bool update(Character character)
        {
            Debug.LogWarning("Strange call to AiRule.update() should never happen. Character: " + character.getName());
            return true;
        }
    }
}