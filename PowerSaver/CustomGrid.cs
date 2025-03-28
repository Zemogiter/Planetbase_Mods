using System;
using System.Collections.Generic;
using System.Linq;
using Planetbase;
using PlanetbaseModUtilities;

namespace PowerSaver
{
    public abstract class CustomGrid : Grid
    {
        public void CalculateBalance(GridResource gridResource)
        {
            bool consoleExists = false;
            foreach (ConstructionComponent component in BuildableUtils.GetAllComponents())
            {
                if (component.getComponentType().GetType() == typeof(GridManagementConsole) && component.isBuilt() && component.isEnabled() && !component.isDestroyed() && component.getParentConstruction().isBuilt() && component.getParentConstruction().isEnabled() && !component.getParentConstruction().isExtremelyDamaged())
                {
                    consoleExists = true;
                    break;
                }
            }

            if (consoleExists)
                AdvancedCalculateBalance(gridResource);
            else
                BasicCalculateBalance(gridResource);
        }

        public void AdvancedCalculateBalance(GridResource gridResource)
        {
            Dictionary<Type, List<Module>> constructionsByType = [];
            float resourceBalance = 0f;
            float amountCreated = 0f;
            float amountConsumed = 0f;

            foreach (Construction construction in Module.getCategoryModules(Module.Category.Count))
            {
                if (construction.isBuilt() && construction.isEnabled() && !construction.isExtremelyDamaged())
                {
                    // amountGenerated can be either created or consumed
                    float amountGenerated = getGeneration(gridResource);
                    resourceBalance += amountGenerated;

                    if (amountGenerated > 0f)
                        amountCreated += amountGenerated;
                    else
                        amountConsumed -= amountGenerated;

                    //setResourceAvailable(construction, gridResource, true);

                    if (construction is Connection)
                        continue;

                    Module module = construction as Module;
                    if (constructionsByType.TryGetValue(module.getModuleType().GetType(), out List<Module> list))
                    {
                        list.Add(module);
                    }
                    else
                    {
                        list = [module];
                        constructionsByType[module.getModuleType().GetType()] = list;
                    }
                }
                else
                {
                    //setResourceAvailable(construction, gridResource, false);
                }
            }

            GridResourceData resourceData = get(gridResource);
            //resourceData.setCollector(findCollector(gridResource, resourceBalance));
            resourceData.setBalance(resourceBalance);
            resourceData.setGeneration(amountCreated);
            resourceData.setConsumption(amountConsumed);

            if (resourceBalance < 0f && resourceData.getCollector() == null)
            {
                Module consoleModule = null;
                List<Type> priorityList = gridResource == GridResource.Power ? PowerSaver.mPowerPriorityList : PowerSaver.mWaterPriorityList;
                foreach (Type type in priorityList)
                {
                    if (constructionsByType.TryGetValue(type, out List<Module> constructions))
                    {
                        foreach (Module module in constructions)
                        {
                            if (consoleModule == null && module.getModuleType() is ModuleTypeControlCenter)
                            {
                                if (module.getComponents().FirstOrDefault(c => c.getComponentType() is GridManagementConsole) != null)
                                {
                                    consoleModule = module;
                                    continue;
                                }
                            }

                            float generation = getGeneration(gridResource);
                            if (generation < 0f && module.isEnabled())
                            {
                                resourceBalance -= generation;
                                //setResourceAvailable(module, gridResource, false);

                                if (resourceBalance > 0f)
                                    return;

                                foreach (Construction connection in module.getLinks())
                                {
                                    bool isResourceAvailable = CoreUtils.InvokeMethod<Grid, bool>("isResourceAvailable", this, connection, gridResource);
                                    if (isResourceAvailable)
                                    {
                                        CoreUtils.InvokeMethod<Grid, float>("getGeneration", this, [connection, gridResource]);
                                        //generation = getGeneration(connection, gridResource);
                                        resourceBalance -= generation;
                                        CoreUtils.InvokeMethod<Grid>("setResourceAvailable", this, [connection, gridResource, false]);

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
                {
                    //setResourceAvailable(consoleModule, gridResource, false);
                    CoreUtils.InvokeMethod<Grid>("setResourceAvailable", this, [consoleModule, gridResource, false]);
                }
            }
        }

        public void BasicCalculateBalance(GridResource gridResource)
        {
            HashSet<Construction> constructionsLackingResource = [];
            //GridResourceData resourceData = Grid.getData(gridResource);
            GridResourceData resourceData = CoreUtils.InvokeMethod<Grid, GridResourceData>("getData", this, gridResource);
            float resourceBalance = 0f;
            float amountCreated = 0f;
            float amountConsumed = 0f;

            foreach (Construction construction in BuildableUtils.GetAllConstructions())
            {
                if (construction.isBuilt() && construction.isEnabled() && !construction.isExtremelyDamaged())
                {
                    // amountGenerated can be either created or consumed
                    float amountGenerated = getGeneration(gridResource);
                    resourceBalance += amountGenerated;

                    if (amountGenerated > 0f)
                        amountCreated += amountGenerated;
                    else
                        amountConsumed -= amountGenerated;

                    bool isResourceAvailable = CoreUtils.InvokeMethod<Grid, bool>("isResourceAvailable", this, construction, gridResource);
                    if (!isResourceAvailable)
                        constructionsLackingResource.Add(construction);

                    CoreUtils.InvokeMethod<Grid>("setResourceAvailable", this, [construction, gridResource, false]);
                    //setResourceAvailable(construction, gridResource, true);
                }
                else
                {
                    //setResourceAvailable(construction, gridResource, false);
                    CoreUtils.InvokeMethod<Grid>("setResourceAvailable", this, [construction, gridResource, false]);
                }
            }

            // if resourceBalance is positive, returns the first collector that is not full.
            // Otherwise, returns the first collector that has available resource
            //Construction collector = findCollector(gridResource, resourceBalance);
            Construction collector = CoreUtils.InvokeMethod<Grid, Construction>("findCollector", this, gridResource, resourceBalance);
            if (resourceBalance < 0f && collector == null)
            {
                HashSet<Construction> constructionsToShutDown = [];
                foreach (Construction construction in BuildableUtils.GetAllConstructions())
                {
                    if (getGeneration(gridResource) < 0f && construction.isEnabled())
                    {
                        //setResourceAvailable(construction, gridResource, false);
                        CoreUtils.InvokeMethod<Grid>("setResourceAvailable", this, [construction, gridResource, false]);
                        if (!constructionsToShutDown.Contains(construction))
                            constructionsToShutDown.Add(construction);
                    }
                }

                float amountAvailable = amountCreated;
                bool somethingChanged = true;
                while (somethingChanged)
                {
                    somethingChanged = false;
                    foreach (Construction construction in BuildableUtils.GetAllConstructions())
                    {
                        bool isResourceAvailable = CoreUtils.InvokeMethod<Grid, bool>("isResourceAvailable", this, construction, gridResource);
                        if (getGeneration(gridResource) < 0f && construction.isEnabled() && !isResourceAvailable)
                        {
                            foreach (Construction linkedConstruction in construction.getLinks())
                            {
                                if (linkedConstruction.isPowered() || getGeneration(gridResource) > 0f)
                                {
                                    float absAmountUsed = -getGeneration(gridResource);
                                    if (absAmountUsed < amountAvailable)
                                    {
                                        //setResourceAvailable(construction, gridResource, true);
                                        CoreUtils.InvokeMethod<Grid>("setResourceAvailable", this, [construction, gridResource, false]);
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
                    //addMessage(constructionsToShutDown, gridResource);
                    CoreUtils.InvokeMethod<Grid>("addMessage", this, [constructionsToShutDown, gridResource]);
                }
            }

            resourceData.setCollector(collector);
            resourceData.setBalance(resourceBalance);
            resourceData.setGeneration(amountCreated);
            resourceData.setConsumption(amountConsumed);
        }
    }
}
