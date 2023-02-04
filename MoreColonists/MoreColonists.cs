using Planetbase;
using System;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Reflection;
using HarmonyLib;

namespace MoreColonists
{
	public class MoreColonists : ModBase
	{
		public static int moreColonist;
		public static int vistors = 10;

		public static new void Init(ModEntry modEntry) => InitializeMod(new MoreColonists(), modEntry, "MoreColonists");

		public override void OnInitialized(ModEntry modEntry)
		{
			var path = "./Mods/MoreColonists/config.txt";
			string line;
			System.IO.StreamReader file = new(path);
			line = file.ReadLine();
			line = line.Substring(13);
            moreColonist = int.Parse(line);
            Console.WriteLine("The value of moreColonist is " + moreColonist + " of type " + moreColonist.GetType());
        }
		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{

		}
    }
	[HarmonyPatch(typeof(VisitorShip), nameof(VisitorShip.onLandedGeneric))]
	public class VisitorShipPatch : VisitorShip
    {
		public static void Postfix(VisitorShip __instance)
		{
            float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
            int num = 10;
            if (MoreColonists.moreColonist != 0)
            {
                num = MoreColonists.moreColonist;
            }
            if (value > 0.9f)
            {
                num += Random.Range(2, 4);
            }
            else if (value > 0.7f)
            {
                num += Random.Range(1, 3);
            }
            if (__instance.mSize == Size.Large)
            {
                num *= 2;
            }
            if (__instance.mIntruders)
            {
                num += LandingShipManager.getExtraIntruders();
                for (int i = 0; i < num; i++)
                {
                    Character.create(TypeList<Specialization, SpecializationList>.find<Intruder>(), __instance.getPosition(), Location.Exterior);
                    CoreUtils.SetMember<VisitorShip, int>("mPendingVisitors", __instance, 0);
                }
                return;
            }
            if (__instance.mIntruders)
            {
                num += LandingShipManager.getExtraIntruders();
                for (int i = 0; i < num; i++)
                {
                    Character.create(TypeList<Specialization, SpecializationList>.find<Intruder>(), __instance.getPosition(), Location.Exterior);
                }
                return;
            }
            
            for (int j = 0; j < num; j++)
            {
                Guest guest = (Guest)Character.create(TypeList<Specialization, SpecializationList>.find<Visitor>(), __instance.getSpawnPosition(j), Location.Exterior);
                guest.decayIndicator(CharacterIndicator.Nutrition, Random.Range(0f, 0.75f));
                guest.decayIndicator(CharacterIndicator.Morale, Random.Range(0f, 1f));
                guest.decayIndicator(CharacterIndicator.Hydration, Random.Range(0f, 0.75f));
                guest.decayIndicator(CharacterIndicator.Sleep, Random.Range(0f, 0.75f));
                guest.setFee(5 * Random.Range(2, 5));
                guest.setOwnedShip(__instance);
                if (Random.Range(0, 20) == 0)
                {
                    guest.setCondition(TypeList<ConditionType, ConditionTypeList>.find<ConditionFlu>());
                }
            }
        }
	}
    [HarmonyPatch(typeof(ColonistShip), nameof(ColonistShip.onLanded))]
    public class ColonistShipPatch : ColonistShip
    {
        public static void Postfix(ColonistShip __instance)
        {
            float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
            int num = 10;
            if (value > 0.9f)
            {
                num += Random.Range(2, 4);
            }
            else if (value > 0.7f)
            {
                num += Random.Range(1, 3);
            }
            if (__instance.mSize == Size.Large)
            {
                num *= 2;
            }
            if (__instance.mIntruders)
            {
                num += LandingShipManager.getExtraIntruders();
            }
            
            for (int i = 0; i < num; i++)
            {
                MethodInfo getMethod = __instance.GetType().GetMethod("calculateSpecialization", BindingFlags.NonPublic | BindingFlags.Instance);
                var calculation = getMethod.Invoke(__instance, new object[] {});
                Specialization specialization = ((Specialization)((!__instance.mIntruders) ? calculation : TypeList<Specialization, SpecializationList>.find<Intruder>()));
                if (specialization != null)
                {
                    Character.create(specialization, __instance.getSpawnPosition(i), Location.Exterior);
                }
            }
        }
    }
}
