using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace BioReactor
{
    [HarmonyPatch(typeof(Construction), "findDamaged")]
    internal class findDamagedPatch
    {
        public static bool Prefix(ref Construction __result, Character character, Specialization specialization)
        {
            float num = float.MaxValue;
            Construction construction = null;
            List<Construction> member = CoreUtils.GetMember<Construction, List<Construction>>("mConstructions");
            for (int i = 0; i < member.Count; i++)
            {
                Construction construction2 = member[i];
                if ((!(construction2 is Module module) || !(module.getModuleType() is ModuleTypeBioReactor)) && construction2.isDamaged(specialization) && construction2.isBuilt() && construction2.anyBuiltLinks() && construction2.getPotentialUserCount(character) == 0)
                {
                    float num2 = (construction2.getPosition() - character.getPosition()).magnitude;
                    if (construction2.isHighPriority())
                    {
                        num2 -= 100f;
                    }
                    Indicator member2 = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", construction2);
                    if (member2.isVeryLow())
                    {
                        num2 -= 100f;
                    }
                    else if (member2.isExtremelyLow())
                    {
                        num2 -= 200f;
                    }
                    if (num2 < num)
                    {
                        num = num2;
                        construction = construction2;
                    }
                }
            }
            __result = construction;
            return false;
        }
    }
    [HarmonyPatch(typeof(Construction), "findExtremelyDamagedInterior")]
    internal class findExtremelyDamagedInteriorPatch
    {
        public static bool Prefix(ref Construction __result, Character character)
        {
            float num = float.MaxValue;
            Construction construction = null;
            List<Construction> member = CoreUtils.GetMember<Construction, List<Construction>>("mConstructions");
            for (int i = 0; i < member.Count; i++)
            {
                Construction construction2 = member[i];
                if ((!(construction2 is Module module) || !(module.getModuleType() is ModuleTypeBioReactor)) && construction2.isExtremelyDamaged() && construction2.isBuilt() && construction2.getLocation() == Location.Interior && construction2.anyBuiltLinks() && construction2.getPotentialUserCount(character) == 0)
                {
                    float magnitude = (construction2.getPosition() - character.getPosition()).magnitude;
                    if (magnitude < num)
                    {
                        num = magnitude;
                        construction = construction2;
                    }
                }
            }
            __result = construction;
            return false;
        }
    }
    [HarmonyPatch(typeof(ConstructionComponent), "updateAnimation")]
    internal class updateAnimationPatch
    {
        public static bool Prefix(ConstructionComponent __instance)
        {
            if (!(__instance.getComponentType() is StarchBurner or VegetableBurner))
            {
                return true;
            }
            Animation member = CoreUtils.GetMember<ConstructionComponent, Animation>("mAnimation", __instance);
            if (member != null)
            {
                if (__instance.isEnabled())
                {
                    if (!member.isPlaying)
                    {
                        member.Play();
                    }
                }
                else if (member.isPlaying)
                {
                    member.Stop();
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(Construction), "updatePowerDescription")]
    internal class updatePowerDescriptionPatch
    {
        public static bool Prefix(Construction __instance)
        {
            DescriptionItem member = CoreUtils.GetMember<Construction, DescriptionItem>("mPowerDescriptionItem", __instance);
            if (member != null)
            {
                int powerGeneration = __instance.getPowerGeneration();
                int maxPowerGeneration = __instance.getMaxPowerGeneration();
                int powerCollection = __instance.getPowerCollection();
                Texture2D icon;
                string text;
                string tooltip;
                if (powerCollection > 0)
                {
                    icon = ResourceList.StaticIcons.PowerGeneration;
                    text = Util.gridResourceToString(powerCollection);
                    tooltip = StringList.get("tooltip_power_collection", Util.gridResourceToString(powerCollection));
                }
                else if (maxPowerGeneration >= 0)
                {
                    int maxPowerGeneration2 = __instance.getMaxPowerGeneration();
                    icon = ResourceList.StaticIcons.PowerGeneration;
                    text = Util.gridResourceToString(powerGeneration) + " / " + Util.gridResourceToString(maxPowerGeneration2);
                    tooltip = StringList.get("tooltip_power_generation", Util.powerToString(powerGeneration), Util.powerToString(maxPowerGeneration2));
                }
                else
                {
                    int componentPowerGeneration = getComponentPowerGeneration(__instance);
                    icon = ((componentPowerGeneration <= 0) ? ResourceList.StaticIcons.PowerConsumption : ResourceList.StaticIcons.PowerGeneration);
                    text = Util.gridResourceToString(Math.Abs(powerGeneration));
                    tooltip = ((componentPowerGeneration >= 0) ? StringList.get("tooltip_power_consumption", Util.powerToString(-powerGeneration)) : StringList.get("tooltip_power_consumption_components", Util.powerToString(-powerGeneration), Util.powerToString(-componentPowerGeneration)));
                }
                member.setText(text);
                member.setTooltip(tooltip);
                member.setIcon(icon);
            }
            return false;
        }

        private static int getComponentPowerGeneration(Construction construction)
        {
            int num = 0;
            List<ConstructionComponent> components = construction.getComponents();
            int count = components.Count;
            for (int i = 0; i < count; i++)
            {
                ConstructionComponent constructionComponent = components[i];
                if (constructionComponent.isBuilt() && constructionComponent.isEnabled())
                {
                    num += constructionComponent.getComponentType().getPowerGeneration();
                }
            }
            return num;
        }
    }
    [HarmonyPatch(typeof(ConstructionComponent), "updateProduction")]
    internal class updateProductionPatch
    {
        public static bool Prefix(ConstructionComponent __instance, float timeStep)
        {
            ComponentType componentType = __instance.getComponentType();
            if (!(componentType is StarchBurner or VegetableBurner))
            {
                return true;
            }
            bool num = enabled(__instance);
            ComponentType componentType2 = (num ? TypeList<ComponentType, ComponentTypeList>.find<StarchBurner>() : TypeList<ComponentType, ComponentTypeList>.find<VegetableBurner>());
            if (__instance.getComponentType() != componentType2)
            {
                CoreUtils.SetMember("mComponentType", __instance, componentType2);
                CoreUtils.InvokeMethod("updateDescriptionItems", __instance);
            }
            if (!num)
            {
                return false;
            }
            float num2 = 1f;
            float resourceProductionPeriod = componentType.getResourceProductionPeriod();
            Indicator member = CoreUtils.GetMember<ConstructionComponent, Indicator>("mProductionProgress", __instance);
            if (member.increase(num2 * timeStep / resourceProductionPeriod))
            {
                ResourceContainer resourceContainer = __instance.getResourceContainer();
                consumeResources(componentType.getResourceConsumption(), resourceContainer);
                member.setValue(0f);
            }
            Construction parentConstruction = __instance.getParentConstruction();
            Module module = parentConstruction as Module;
            Indicator member2 = CoreUtils.GetMember<Construction, Indicator>("mConditionIndicator", parentConstruction);
            float num3 = timeStep / module.getModuleType().getConditionDecayTime(module.getSizeIndex());
            if (module.hasWater())
            {
                if (__instance.anyInteractions())
                {
                    member2.increase(num3);
                }
                else
                {
                    member2.decrease(num3);
                }
            }
            else
            {
                member2.decrease(num3 * 5f);
            }
            return false;
        }

        private static bool enabled(ConstructionComponent __instance)
        {
            if (!__instance.getParentConstruction().isEnabled())
            {
                return false;
            }
            if (CoreUtils.GetMember<ConstructionComponent, Indicator>("mConditionIndicator", __instance).isExtremelyLow())
            {
                return false;
            }
            if (!__instance.getComponentType().isProducer())
            {
                return false;
            }
            if (!__instance.isOperational())
            {
                return false;
            }
            if (!__instance.canProduce())
            {
                return false;
            }
            if (__instance.getResourceContainer().getResourceCount() == 0)
            {
                return false;
            }
            return true;
        }

        private static void consumeResources(List<ResourceType> resourceConsumption, ResourceContainer mResourceContainer)
        {
            int highestCountResourceIndex = Resource.getHighestCountResourceIndex(resourceConsumption);
            for (int i = 0; i < resourceConsumption.Count; i++)
            {
                ResourceType resourceType = resourceConsumption[(i + highestCountResourceIndex) % resourceConsumption.Count];
                if (mResourceContainer.contains(resourceType))
                {
                    mResourceContainer.remove(resourceType)?.destroy();
                }
            }
        }
    }
}
