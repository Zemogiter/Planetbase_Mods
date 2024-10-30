using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static Planetbase.ModuleType;
using static UnityModManagerNet.UnityModManager;

namespace NoIntruders
{
    
    public class NoIntruders : ModBase
    {
		public static new void Init(ModEntry modEntry) => InitializeMod(new NoIntruders(), modEntry, "NoIntruders");

		public override void OnInitialized(ModEntry modEntry)
		{

		}

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{

        }
	}
	[HarmonyPatch(typeof(Character), nameof(Character.getFirstCharacter))]
	public class CharacterPatch
	{
		public static void Postfix(Character __instance)
		{
            List<Character> intruders = Character.getSpecializationCharacters(SpecializationList.IntruderInstance);
            var assemblyList = Assembly.GetExecutingAssembly().GetTypes().Select(t => t.Namespace).ToArray();
            string assemblyToFind = "MoreColonists";
            var result = Array.Find(assemblyList, element => element == assemblyToFind);
            if (intruders != null && result == null)
            {
                Console.WriteLine("More Colonists mod not found, can execute the CharacterPatch.Postfix without risk of a crash");
                foreach (Character intruder in intruders)
                {
                    // kill intruders instantly.
                    if (intruder != null && !intruder.isDead())
                    {
                        intruder.setArmed(false);
						CoreUtils.InvokeMethod<Character>("setDead", __instance);
                    }
                }
            }
            else
            {
                Console.WriteLine("More Colonists mod found, can't run CharacterPatch.Postfix.");
                return;
            }
        }
	}
	[HarmonyPatch(typeof(Planet), nameof(Planet.getIntruderMinPrestige))]
	public class PlanetPatch
	{
		public static bool Prefix(Planet __instance)
		{
            CoreUtils.SetMember("mIntruderMinPrestige", __instance, 2000f);
			Console.WriteLine("Minimum prestige for intruders to appear is = " + __instance.mIntruderMinPrestige.ToString());
            return false;
        }
	}
}
