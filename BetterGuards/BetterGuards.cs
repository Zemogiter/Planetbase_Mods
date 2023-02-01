using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Selectable = Planetbase.Selectable;
using HarmonyLib;
using System.Data;

namespace BetterGuards
{
    public class BetterGuards : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new BetterGuards(), modEntry, "BetterGuards");

        public static float healthmult;

        public override void OnInitialized(ModEntry modEntry)
        {
            var path = "./Mods/BetterGuards/config.txt";
            string line;
            System.IO.StreamReader file = new(path);
            line = file.ReadLine();
            line = line.Substring(14);
            healthmult = float.Parse(line);
            Console.WriteLine("The value of healthmult is " + healthmult + " of type " + healthmult.GetType());
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    [HarmonyPatch(typeof(Human), nameof(Human.init))]
    public class GuardPatch
    {
        //replacing the default health bar with a larger one for guards
        public static void Postfix(Character __instance)
        {
            if (__instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>())
            {
                Indicator indicator = new(StringList.get("health"), ResourceList.StaticIcons.Health, IndicatorType.Normal, 1f, BetterGuards.healthmult, SignType.Health);
                indicator.setLevels(0.1f, 0.5f, 0.7f, 0.8f);
                indicator.setOrientation(IndicatorOrientation.Vertical);
                //__instance.mIndicators[0] = indicator;
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.onKo))]
    public class CharacterPatch
    {
        //make guards and intruders drop guns on death (vanilla = guns disapear with guards/intruders)
        public static void Postfix(Character __instance)
        {
            if(__instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>() || __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
            {
                Vector3 position = __instance.getPosition();
                Location location = __instance.getLocation();
                ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();
                Resource droppedGun = Resource.create(gunType, position, location);
            }
        }
    }
}
