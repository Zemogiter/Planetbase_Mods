using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSaver
{
    public abstract class CustomGrid : Grid
    {
        public new void calculateBalance(GridResource gridResource)
        {
            bool consoleExists = false;
            foreach (ConstructionComponent component in ConstructionComponent.mComponents)
            {
                bool lowCondition = component.mConditionIndicator.isValidValue() && component.mConditionIndicator.isExtremelyLow();
                if (component.getComponentType().GetType() == typeof(GridManagementConsole) && component.isBuilt() && !lowCondition && component.isEnabled() && component.mParentConstruction.isBuilt() && component.mParentConstruction.isEnabled() && !component.mParentConstruction.isExtremelyDamaged())
                {
                    consoleExists = true;
                    break;
                }
            }

            if (consoleExists)
                advancedCalculateBalance(gridResource);
            else
                basicCalculateBalance(gridResource);
        }

        public void advancedCalculateBalance(GridResource gridResource)
        {
            Dictionary<Type, List<Module>> constructionsByType = new Dictionary<Type, List<Module>>();
            float resourceBalance = 0f;
            float amountCreated = 0f;
            float amountConsumed = 0f;

            foreach (Construction construction in mConstructions)
            {
                if (construction.isBuilt() && construction.isEnabled() && !construction.isExtremelyDamaged())
                {
                    // amountGenerated can be either created or consumed
                    float amountGenerated = getGeneration(construction, gridResource);
                    resourceBalance += amountGenerated;

                    if (amountGenerated > 0f)
                        amountCreated += amountGenerated;
                    else
                        amountConsumed -= amountGenerated;

                    setResourceAvailable(construction, gridResource, true);

                    if (construction is Connection)
                        continue;

                    Module module = construction as Module;
                    List<Module> list;
                    if (constructionsByType.TryGetValue(module.mModuleType.GetType(), out list))
                    {
                        list.Add(module);
                    }
                    else
                    {
                        list = new List<Module>();
                        list.Add(module);
                        constructionsByType[module.mModuleType.GetType()] = list;
                    }
                }
                else
                {
                    setResourceAvailable(construction, gridResource, false);
                }
            }

            GridResourceData resourceData = getData(gridResource);
            resourceData.setCollector(findCollector(gridResource, resourceBalance));
            resourceData.setBalance(resourceBalance);
            resourceData.setGeneration(amountCreated);
            resourceData.setConsumption(amountConsumed);

            if (resourceBalance < 0f && resourceData.getCollector() == null)
            {
                Module consoleModule = null;
                List<Type> priorityList = gridResource == GridResource.Power ? PowerSaver.mPowerPriorityList : PowerSaver.mWaterPriorityList;
                foreach (Type type in priorityList)
                {
                    List<Module> constructions;
                    if (constructionsByType.TryGetValue(type, out constructions))
                    {
                        foreach (Module module in constructions)
                        {
                            if (consoleModule == null && module.mModuleType is ModuleTypeControlCenter)
                            {
                                if (module.mComponents.FirstOrDefault(c => c.mComponentType is GridManagementConsole) != null)
                                {
                                    consoleModule = module;
                                    continue;
                                }
                            }

                            float generation = getGeneration(module, gridResource);
                            if (generation < 0f && module.isEnabled())
                            {
                                resourceBalance -= generation;
                                setResourceAvailable(module, gridResource, false);

                                if (resourceBalance > 0f)
                                    return;

                                foreach (Construction connection in module.getLinks())
                                {
                                    if (isResourceAvailable(connection, gridResource))
                                    {
                                        generation = getGeneration(connection, gridResource);
                                        resourceBalance -= generation;
                                        setResourceAvailable(connection, gridResource, false);

                                        if (resourceBalance > 0f)
                                            return;
                                    }
                                }
                            }
                        }
                    }
                }

                // if we reach this point, we still don't have a positive balance
                // and the only module active is the control center with the grid management console
                if (consoleModule != null)
                    setResourceAvailable(consoleModule, gridResource, false);
            }
        }

        public void basicCalculateBalance(GridResource gridResource)
        {
            HashSet<Construction> constructionsLackingResource = new HashSet<Construction>();
            GridResourceData resourceData = this.getData(gridResource);
            float resourceBalance = 0f;
            float amountCreated = 0f;
            float amountConsumed = 0f;

            foreach (Construction construction in mConstructions)
            {
                if (construction.isBuilt() && construction.isEnabled() && !construction.isExtremelyDamaged())
                {
                    // amountGenerated can be either created or consumed
                    float amountGenerated = getGeneration(construction, gridResource);
                    resourceBalance += amountGenerated;

                    if (amountGenerated > 0f)
                        amountCreated += amountGenerated;
                    else
                        amountConsumed -= amountGenerated;

                    if (!isResourceAvailable(construction, gridResource))
                        constructionsLackingResource.Add(construction);

                    setResourceAvailable(construction, gridResource, true);
                }
                else
                {
                    setResourceAvailable(construction, gridResource, false);
                }
            }

            // if resourceBalance is positive, returns the first collector that is not full.
            // Otherwise, returns the first collector that has available resource
            Construction collector = findCollector(gridResource, resourceBalance);

            if (resourceBalance < 0f && collector == null)
            {
                HashSet<Construction> constructionsToShutDown = new HashSet<Construction>();
                foreach (Construction construction in mConstructions)
                {
                    if (getGeneration(construction, gridResource) < 0f && construction.isEnabled())
                    {
                        setResourceAvailable(construction, gridResource, false);
                        if (!constructionsToShutDown.Contains(construction))
                            constructionsToShutDown.Add(construction);
                    }
                }

                float amountAvailable = amountCreated;
                bool somethingChanged = true;
                while (somethingChanged)
                {
                    somethingChanged = false;
                    foreach (Construction construction in mConstructions)
                    {
                        if (getGeneration(construction, gridResource) < 0f && construction.isEnabled() && !isResourceAvailable(construction, gridResource))
                        {
                            foreach (Construction linkedConstruction in construction.getLinks())
                            {
                                if (linkedConstruction.isPowered() || getGeneration(linkedConstruction, gridResource) > 0f)
                                {
                                    float absAmountUsed = -getGeneration(construction, gridResource);
                                    if (absAmountUsed < amountAvailable)
                                    {
                                        setResourceAvailable(construction, gridResource, true);
                                        if (constructionsToShutDown.Contains(construction))
                                        {
                                            constructionsToShutDown.Remove(construction);
                                        }
                                        amountAvailable -= absAmountUsed;
                                        somethingChanged = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                if ((gridResource == GridResource.Water && !MessageLog.getInstance().contains(Message.StructuresNoWater)) ||
                    (gridResource == GridResource.Power && !MessageLog.getInstance().contains(Message.StructuresNoPower)))
                {
                    constructionsToShutDown.UnionWith(constructionsLackingResource);
                    addMessage(constructionsToShutDown, gridResource);
                }
            }

            resourceData.setCollector(collector);
            resourceData.setBalance(resourceBalance);
            resourceData.setGeneration(amountCreated);
            resourceData.setConsumption(amountConsumed);
        }
    }
}
