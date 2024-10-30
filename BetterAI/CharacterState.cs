using System;
using System.Collections.Generic;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace BetterAI
{
    public class CharacterState : ScheduledState
    {
        Dictionary<CONDITION, CharacterIndicator> mCharacterIndicators;
        Dictionary<CONDITION, CharacterIndicator> mCharacterIndicatorsCrit;

        public CharacterState() : base()
        {
            mCharacterIndicators = new Dictionary<CONDITION, CharacterIndicator>();
            if (mCharacter is Human)
            {
                mCharacterIndicators.Add(CONDITION.LOW_OXYGEN, CharacterIndicator.Oxygen);
                mCharacterIndicators.Add(CONDITION.LOW_HEALTH, CharacterIndicator.Health);
                mCharacterIndicators.Add(CONDITION.LOW_HYDRATION, CharacterIndicator.Hydration);
                mCharacterIndicators.Add(CONDITION.LOW_NUTRITION, CharacterIndicator.Nutrition);
                mCharacterIndicators.Add(CONDITION.LOW_SLEEP, CharacterIndicator.Sleep);
                mCharacterIndicators.Add(CONDITION.LOW_MORALE, CharacterIndicator.Morale);
            }
            else if (mCharacter is Bot)
            {
                mCharacterIndicators.Add(CONDITION.LOW_CONDITION, CharacterIndicator.Condition);
                mCharacterIndicators.Add(CONDITION.LOW_INTEGRITY, CharacterIndicator.Integrity);
            }

            mCharacterIndicatorsCrit = new Dictionary<CONDITION, CharacterIndicator>();

            if (mCharacter is Human)
            {
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_OXYGEN, CharacterIndicator.Oxygen);
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_HEALTH, CharacterIndicator.Health);
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_HYDRATION, CharacterIndicator.Hydration);
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_NUTRITION, CharacterIndicator.Nutrition);
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_SLEEP, CharacterIndicator.Sleep);
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_MORALE, CharacterIndicator.Morale);
            }
            else if (mCharacter is Bot)
            {
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_CONDITION, CharacterIndicator.Condition);
                mCharacterIndicatorsCrit.Add(CONDITION.CRIT_INTEGRITY, CharacterIndicator.Integrity);
            }
        }

        //=========================================================
        // RunAI
        //=========================================================
        public void RunAI()
        {
            // get wise
            GatherConditions();

            // use the schwartz lonestar!
            MaintainSchedule();

            // remove all interrupt conditions, for good measure
            //if (HasSchedule())
            //    RemoveCondition(mSchedule.mInterruptConditions);
        }

        //=========================================================
        // GatherConditions - Look, Hear, Smell
        //=========================================================
        private void GatherConditions()
        {
            //Debug.Log("gathering conditions");

            // start with fresh conditions
            ClearConditions();

            // no long running code in these,
            // only fast condition gathering
            GatherSurvivalConditions();
            GatherSecurityConditions();
            GatherMaterialsConditions();

            // todo

            Debug.Log("conditions mask: " + mConditions.ToString());
        }

        private void GatherSurvivalConditions()
        {
            Character character = new NewCharacter();
            // check low indicators
            foreach (KeyValuePair<CONDITION, CharacterIndicator> kvp in mCharacterIndicators)
            {
                if (mCharacter.isIndicatorLow(kvp.Value))
                    AddCondition(kvp.Key);
            }

            // now for critical states
            foreach (KeyValuePair<CONDITION, CharacterIndicator> kvp in mCharacterIndicatorsCrit)
            {
                if (mCharacter.isStatusExtremelyLow(kvp.Value))
                    AddCondition(kvp.Key);
            }

            // Are we outdoors?
            if (mCharacter.getLocation() == Location.Exterior)
                AddCondition(CONDITION.IS_EXTERIOR);

            if (mCharacter.isLoaded())
                AddCondition(CONDITION.IS_LOADED);

            bool characterRestoration = CoreUtils.InvokeMethod<Character, bool>("isBeingRestored", character, null);
            bool miningCheck = CoreUtils.InvokeMethod<Character, bool>("isMining", character, null);
            if (characterRestoration)
                AddCondition(CONDITION.IS_BEING_RESTORED);

            if (mCharacter.isFitForWork())
                AddCondition(CONDITION.IS_FIT_FOR_WORK);

            if (mCharacter.isFitForCriticalWork())
                AddCondition(CONDITION.IS_FIT_FOR_CRITICAL_WORK);

            if (miningCheck)
                AddCondition(CONDITION.IS_MINING);

            if (!(mCharacter is Bot))
            {
                if (!Bot.anyFree(Specialization.FlagCarrier))
                    AddCondition(CONDITION.NO_BOT_CARRIER);

                if (!Bot.anyFree(Specialization.FlagBuilder))
                    AddCondition(CONDITION.NO_BOT_BUILDER);

                if (!Bot.anyFree(Specialization.FlagMiner))
                    AddCondition(CONDITION.NO_BOT_MINER);
            }

        }

        private void GatherSecurityConditions()
        {
            // Are there enemies?
            int maxTargets = 5;
            float lookDistance = 200;
            if (mCharacter.getSpecialization().hasFlag(Specialization.FlagSecurity))
            {
                maxTargets += 2;
                lookDistance += 200;
            }
            if (mCharacter.hasStatusFlag(Character.StatusFlagArmed))
            {
                maxTargets += 2;
                lookDistance += 200;
            }

            Human human = Human.findNearestDetectedIntruder(mCharacter, maxTargets, lookDistance);
            if (human != null)
            {
                if (!human.isDead())
                    AddCondition(CONDITION.SEE_ENEMY);
                else
                    AddCondition(CONDITION.ENEMY_DEAD);
            }

            // are we armed, aggressive, detected?
            if (mCharacter.hasStatusFlag(Character.StatusFlagArmed))
                AddCondition(CONDITION.HAS_WEAPON);

            if (mCharacter.hasStatusFlag(Character.StatusFlagAggressive))
                AddCondition(CONDITION.IS_AGGRESSIVE);

            if (mCharacter.hasStatusFlag(Character.StatusFlagDetected))
                AddCondition(CONDITION.IS_DETECTED);

            SecurityManager sm = Singleton<SecurityManager>.getInstance();
            if (sm.getAlertState() == AlertState.RedAlert)
                AddCondition(CONDITION.RED_ALERT);
            else if (sm.getAlertState() == AlertState.YellowAlert)
                AddCondition(CONDITION.YELLOW_ALERT);
            else
                AddCondition(CONDITION.FREE_TO_GO_OUTSIDE);
        }

        private void GatherMaterialsConditions()
        {
            if (Resource.getCountOfType((ResourceType)ResourceTypeList.SparesInstance) == 0)
                AddCondition(CONDITION.NO_SPARES);

            if (Ship.getFirstOfType<MerchantShip>() != null)
                AddCondition(CONDITION.MERCHANT_AWAITING_MATERIALS);

            Resource traded = Resource.findTraded(mCharacter);
            if (traded != null)
                AddCondition(CONDITION.MERCHANT_MATERIALS_AVAILABLE);

            IList<Buildable> awaitingMaterials = Buildable.findAwaitingMaterials(mCharacter);
            if (awaitingMaterials.Count > 0)
            {
                AddCondition(CONDITION.CONSTRUCTION_AWAITING_MATERIALS);

                for (int index1 = 0; index1 < awaitingMaterials.Count; ++index1)
                {
                    Buildable buildable = awaitingMaterials[index1];
                    ResourceAmounts constructionCosts = buildable.getPredictedPendingConstructionCosts(mCharacter);
                    if (constructionCosts != null && !constructionCosts.isEmpty())
                    {
                        int index2 = UnityEngine.Random.Range(0, constructionCosts.getCount());
                        ResourceType resourceType = constructionCosts.get(index2).getResourceType();
                        Resource available = Resource.findAvailable(mCharacter, buildable.getPosition(), buildable.getBuildLocation(), resourceType, false);
                        if (available != null)
                        {
                            AddCondition(CONDITION.CONSTRUCTION_MATERIALS_AVAILABLE);
                            break;
                        }
                    }
                }
            }

            TransformerList transformers = ConstructionComponent.findTransformers(mCharacter);
            for (int i = 0; i < transformers.getCount(); ++i)
            {
                TransformerEntry transformer = transformers.getTransformer(i);
                if (Singleton<ManufactureLimits>.getInstance().isUnderLimit(transformer.getTransformer()))
                {
                    if (!HasCondition(CONDITION.TRANSFORMER_AWAITING_MATERIALS))
                        AddCondition(CONDITION.TRANSFORMER_AWAITING_MATERIALS);

                    List<ResourceType> predictedNeededResources = transformer.getPredictedNeededResources();
                    int countResourceIndex = Resource.getHighestCountResourceIndex(predictedNeededResources);
                    for (int index = 0; index < predictedNeededResources.Count; ++index)
                    {
                        ResourceType resourceType = predictedNeededResources[(index + countResourceIndex) % predictedNeededResources.Count];
                        Resource available = Resource.findAvailable(mCharacter, transformer.getTransformer().getPosition(), Location.Interior, resourceType, false);
                        if (available != null)
                        {
                            AddCondition(CONDITION.TRANSFORMER_MATERIALS_AVAILABLE);
                            break;
                        }
                    }
                }
                if (HasCondition(CONDITION.TRANSFORMER_MATERIALS_AVAILABLE))
                    break;
            }

            Resource storable;

            foreach (ResourceType resourceType in ResourceTypeList.getInstance().mTypeList)
            {
                ConstructionComponent storageComponent = Module.findStorageComponent(mCharacter, resourceType);
                if (storageComponent != null)
                {
                    storable = Resource.findStorable(mCharacter, resourceType, true);
                    if (storable != null)
                        AddCondition(CONDITION.MATERIALS_AVAILABLE_FOR_STORAGECOMPONENT);
                    break;
                }
            }

            Module storage = Module.findStorage(mCharacter);
            if (storage != null)
            {
                AddCondition(CONDITION.STORAGE_AVAILABLE_FOR_ITEMS);

                storable = Resource.findStorable(mCharacter, (ResourceType)null, true);
                if (storable != null)
                    AddCondition(CONDITION.MATERIALS_AVAILABLE_FOR_STORAGE);
            }
        }

        //=========================================================
        // Helper - 
        //=========================================================
    }
}
