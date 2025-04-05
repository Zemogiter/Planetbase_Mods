using HarmonyLib;
using Planetbase;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
using PlanetbaseModUtilities;

namespace MoreColonists.Patches
{
    //main code for colonists
    [HarmonyPatch(typeof(ColonistShip), nameof(ColonistShip.onLanded))]
    public class ColonistShipPatch : ColonistShip
    {
        public static void Postfix(ColonistShip __instance)
        {
            float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
            int num = MoreColonists.settings.moreColonist;
            if (value > 0.9f)
            {
                num += Random.Range(4, 7);
            }
            else if (value > 0.7f)
            {
                num += Random.Range(2, 5);
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
                }
            }

            for (int i = 0; i < num; i++)
            {
                MethodInfo getMethod = __instance.GetType().GetMethod("calculateSpecialization", BindingFlags.NonPublic | BindingFlags.Instance);
                var calculation = getMethod.Invoke(__instance, []);
                Specialization specialization = (Specialization)((!CoreUtils.GetMember<LandingShip, bool>("mIntruders", __instance)) ? calculation : TypeList<Specialization, SpecializationList>.find<Intruder>());
                if (specialization != null)
                {
                    Vector3 spawnPosition = CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, i);
                    Character.create(specialization, spawnPosition, Location.Exterior);
                }
            }
            if (MoreColonists.settings.botColonistsMode == true && Random.Range(0, 20) == 0) //handling extra bots in ColonistShip
            {
                int max = Mathf.RoundToInt(Random.Range(1, 20));
                if (max == 0)
                {
                    max = 1;
                }
                int j;
                for (j = 0; j < max; j++)
                {
                    List<Specialization> randomizer =
                    [
                        TypeList<Specialization, SpecializationList>.find<Driller>(),
                        TypeList<Specialization, SpecializationList>.find<Constructor>(),
                        TypeList<Specialization, SpecializationList>.find<Carrier>()
                    ];
                    Specialization specialization = randomizer[Random.Range(0, randomizer.Count)];
                    Character.create(specialization, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                }
                if (MoreColonists.settings.displayBotColonist == true)
                {
                    Message message = new(StringList.get("New colonists arrived with " + j.ToString() + " bots on board."), ResourceList.StaticIcons.Bot, 8);
                    Singleton<MessageLog>.getInstance().addMessage(message);
                }
            }
        }
    }
}
