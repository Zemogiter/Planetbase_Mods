using Planetbase;
using System;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Reflection;

namespace MoreColonists
{
	public class MoreColonists : ModBase
	{
		public static int newcolonists;
		public static int vistors = 20;

		public static new void Init(ModEntry modEntry) => InitializeMod(new MoreColonists(), modEntry, "MoreColonists");

		public override void OnInitialized(ModEntry modEntry)
		{
			var path = "./Mods/MoreColonists/config.txt";
			string line;
			System.IO.StreamReader file = new(path);
			line = file.ReadLine();
			line = line.Substring(13);
			newcolonists = int.Parse(line);
		}
		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{

			var landedGeneric = new CustomVisitorship();
			landedGeneric.onLandedGeneric();
			var landed = new CustomColonistShip();
			landed.onLanded();
		}

		public class CustomVisitorship : VisitorShip
		{
#pragma warning disable IDE1006 // Naming Styles
			public new void onLandedGeneric(VisitorShip visitorShip)
#pragma warning restore IDE1006 // Naming Styles
			{
				float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
				int num = 10;
				if (newcolonists != 0)
				{
					num = newcolonists;
				}
				if (value > 0.9f)
				{
					num += Random.Range(2, 4);
				}
				else if (value > 0.7f)
				{
					num += Random.Range(1, 3);
				}
				if (mSize == Size.Large)
				{
					num *= 2;
				}
                if (mIntruders)
                {
                    num += LandingShipManager.getExtraIntruders();
                    for (int i = 0; i < num; i++)
                    {
                        Character.create(TypeList<Specialization, SpecializationList>.find<Intruder>(), getPosition(), Location.Exterior);
						CoreUtils.SetMember<VisitorShip, int>("mPendingVisitors", visitorShip, 0);
                    }
                    return;
                }
                try
				{
					VisitorShip instance = new();
					int pendingVisitors = CoreUtils.GetMember<VisitorShip, int>("mPendingVisitors");
					if (mIntruders)
					{
						num += LandingShipManager.getExtraIntruders();
						for (int i = 0; i < num; i++)
						{
							Character.create(TypeList<Specialization, SpecializationList>.find<Intruder>(), getPosition(), Location.Exterior);
							pendingVisitors = 0;
						}
						return;
					}
				}
				catch (NullReferenceException ex)
				{
					Debug.LogError(ex.Message);
				}
				for (int j = 0; j < num; j++)
				{
					Guest guest = (Guest)Character.create(TypeList<Specialization, SpecializationList>.find<Visitor>(), getSpawnPosition(j),Location.Exterior);
					guest.decayIndicator(CharacterIndicator.Nutrition, Random.Range(0f, 0.75f));
					guest.decayIndicator(CharacterIndicator.Morale, Random.Range(0f, 1f));
					guest.decayIndicator(CharacterIndicator.Hydration, Random.Range(0f, 0.75f));
					guest.decayIndicator(CharacterIndicator.Sleep, Random.Range(0f, 0.75f));
					guest.setFee(5 * Random.Range(2, 5));
					guest.setOwnedShip(this);
					if (Random.Range(0, 20) == 0)
					{
						guest.setCondition(TypeList<ConditionType, ConditionTypeList>.find<ConditionFlu>());
					}
				}
			}
		}

		public class CustomColonistShip : ColonistShip
		{
#pragma warning disable IDE1006 // Naming Styles
			public new void onLanded(ColonistShip colonistShip)
#pragma warning restore IDE1006 // Naming Styles
			{
				float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
				int num = 20;
                if (value > 0.9f)
                {
                    num += Random.Range(2, 4);
                }		
				else if (value > 0.7f)
				{
					num += Random.Range(1, 3);
				}
				if (mSize == Size.Large)
				{
					num *= 2;
				}
				if (mIntruders)
				{
					num += LandingShipManager.getExtraIntruders();
				}
				try
				{
					for (int i = 0; i < num; i++)
					{
						MethodInfo getMethod = this.GetType().GetMethod("calculateSpecialization", BindingFlags.NonPublic | BindingFlags.Instance);
						var calculation = getMethod.Invoke(this, new object[] { colonistShip });
						Specialization specialization = ((Specialization)((!mIntruders) ? calculation : TypeList<Specialization, SpecializationList>.find<Intruder>()));
						if (specialization != null)
						{
							Character.create(specialization, getSpawnPosition(i), Location.Exterior);
						}
					}
				}
				catch (NullReferenceException ex2)
				{
					Debug.LogError(ex2.Message);
				}
			}
		}
       
    }
}
