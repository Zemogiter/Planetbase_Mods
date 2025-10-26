using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoreColonists.Patches
{
    // main code for visitors
    [HarmonyPatch(typeof(VisitorShip), nameof(VisitorShip.onLandedGeneric))]
    public class VisitorShipPatch : VisitorShip
    {
        public static void Postfix(VisitorShip __instance)
        {
            float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
            int num = 10;
            if (MoreColonists.settings.moreVisitors != 0)
            {
                num = MoreColonists.settings.moreVisitors;
            }
            if (value > 0.9f)
            {
                num += Random.Range(2, 4);
            }
            else if (value > 0.7f)
            {
                num += Random.Range(1, 3);
            }
            if (CoreUtils.GetMember<LandingShip, Size>("mSize", __instance) == Size.Large)
            {
                num *= 2;
            }
            if (CoreUtils.GetMember<LandingShip, bool>("mIntruders", __instance))
            {
                if (MoreColonists.settings.noIntruders == true)
                {
                    num = 0;
                }
                else
                {
                    num += LandingShipManager.getExtraIntruders();
                    for (int i = 0; i < num; i++)
                    {
                        Character.create(TypeList<Specialization, SpecializationList>.find<Intruder>(), __instance.getPosition(), Location.Exterior);
                        CoreUtils.SetMember<VisitorShip, int>("mPendingVisitors", __instance, 0);
                    }
                    return;
                }
            }
            CoreUtils.SetMember<VisitorShip, int>("mPendingVisitors", __instance, num);
            for (int j = 0; j < num; j++)
            {
                Vector3 spawnPosition = CoreUtils.InvokeMethod<VisitorShip, Vector3>("getSpawnPosition", __instance, j);
                Guest guest = (Guest)Character.create(TypeList<Specialization, SpecializationList>.find<Visitor>(), spawnPosition, Location.Exterior);
                guest.decayIndicator(CharacterIndicator.Nutrition, Random.Range(0f, 0.75f));
                guest.decayIndicator(CharacterIndicator.Morale, Random.Range(0f, 1f));
                guest.decayIndicator(CharacterIndicator.Hydration, Random.Range(0f, 0.75f));
                guest.decayIndicator(CharacterIndicator.Sleep, Random.Range(0f, 0.75f));
                guest.setFee(5 * Random.Range(2, 5));
                guest.setOwnedShip(__instance);
                if (Random.Range(0, 20) == 0 && MoreColonists.settings.canBeCarrier)
                {
                    guest.setCondition(TypeList<ConditionType, ConditionTypeList>.find<ConditionFlu>());
                }
            }
        }
    }
    [HarmonyPatch(typeof(LandingShip), nameof(LandingShip.update))]
    public class VisitorShipFixPatch
    {
        //fix for the issue that makes visitor ships not fly off once the counter reaches zero
        public static void Postfix(VisitorShip __instance)
        {
            if (__instance != null && __instance.isLanded() && __instance.getName() == "Visitor Ship" && (__instance.getPendingVisitorCount() <= 0 || __instance.getDescription() == null))
            {
                Console.WriteLine("MoreColonists - state of the glitched VisitorShip: " + CoreUtils.GetMember<LandingShip, LandingShip.State>("mState", __instance) + " and the name (should be VisitorShip) is: " + __instance.getName());
                CoreUtils.InvokeMethod<LandingShip>("setState", __instance, LandingShip.State.ClosingDoor);
                CoreUtils.InvokeMethod<LandingShip>("setState", __instance, LandingShip.State.TakingOff);
            }
        }
    }
}
