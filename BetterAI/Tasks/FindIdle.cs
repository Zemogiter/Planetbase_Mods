using Planetbase;
using UnityEngine;

namespace BetterAI.Tasks
{
    class FindIdlePosition : AbstractTask
    {
        public override void Start(ScheduledState ai)
        {
            Character character = ai.mCharacter;

            Construction interiorConstruction = Construction.findNearestInteriorConstruction(character.getPosition());
            if (interiorConstruction != null)
            {
                float sqrMagnitude = float.MaxValue;
                Character nearestCharacter = Character.findNearestCharacter(character);
                if (nearestCharacter != null)
                {
                    sqrMagnitude = (nearestCharacter.getPosition() - character.getPosition()).sqrMagnitude;
                }

                bool idlePosition = interiorConstruction.getIdlePosition(out Vector3 position);
                if (character.isWanderTime() || (double)sqrMagnitude < 1.0 || !idlePosition)
                {
                    Target target = (Target)null;
                    if (idlePosition && Random.Range(0, 2) == 0)
                    {
                        target = new Target((Selectable)interiorConstruction, position);
                    }
                    else
                    {
                        int linkCount = interiorConstruction.getLinkCount();
                        if (linkCount > 0)
                        {
                            Construction link = interiorConstruction.getLink(Random.Range(0, linkCount));
                            if (link.isBuilt() && !link.hasFlag(4) && link.getLocation() == Location.Interior)
                                target = !link.getIdlePosition(out position) ? new Target((Selectable)link, link.getRandomWalkablePosition()) : new Target((Selectable)link, position);
                        }
                    }
                    if (target != null && (double)(Character.findNearestCharacter(character, target.getPosition()).getPosition() - character.getPosition()).sqrMagnitude > 1.0)
                    {
                        ai.mMoveTarget = target;
                        ai.CompleteTask();
                        return;
                    }
                }
            }
        }

        public override void Run(ScheduledState ai)
        {
            Start(ai);
        }
    }
}