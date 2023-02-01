using Planetbase;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Collections.Generic;
using static Planetbase.ModuleType;
using System.Reflection;

namespace NoIntruders
{
    
    public class NoIntruders : ModBase
    {
		public static Planet Planet { get; private set; }
		public static new void Init(ModEntry modEntry) => InitializeMod(new NoIntruders(), modEntry, "NoIntruders");

		public override void OnInitialized(ModEntry modEntry)
		{
			
			
		}

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
			FieldInfo field = (FieldInfo)typeof(Planet).GetField("mIntruderMinPrestige", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Planet);
			Console.WriteLine("Minimum prestige for intruders is = " + field.ToString());
        }
	}
	[HarmonyPatch(typeof(Character), nameof(Character.getFirstCharacter))]
	public class CharacterClass
	{
		public static void Postfix()
		{
            List<Character> intruders = Character.getSpecializationCharacters(SpecializationList.IntruderInstance);
            if (intruders != null)
            {
                foreach (Character intruder in intruders)
                {
                    // kill intruders instantly.
                    if (intruder != null && !intruder.isDead())
                    {
                        intruder.setArmed(false);
						//var dead = new CharacterImplementation();
						//dead.SetDead();
						CoreUtils.InvokeMethod<Character>("setDead", intruder);
                    }
                }
            }
        }
	}
	[HarmonyPatch(typeof(Planet), nameof(Planet.getIntruderMinPrestige))]
	public class PlanetPatch
	{
		public static bool Prefix()
		{
			var prestige = PlanetManager.getCurrentPlanet().getIntruderMinPrestige();
            typeof(Planet).GetField("mIntruderMinPrestige", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(prestige, 2000f);
            //CoreUtils.SetMember("mIntruderMinPrestige", NoIntruders.Planet, 2000f);
            return false;
        }
		
	}
	/*public class CharacterImplementation : Character
	{
		public void SetDead()
		{
			base.setDead();
		}
		//Overides below are placeholders to prevent compiler errors
		public override float getHeight()
		{
			throw new NotImplementedException();
		}

		public override Texture2D getIcon()
		{
			throw new NotImplementedException();
		}

		public override Bounds getSelectionBounds()
		{
			throw new NotImplementedException();
		}

		protected override List<string> getAnimationNames(CharacterAnimationType animationType)
		{
			throw new NotImplementedException();
		}
	}*/
}
