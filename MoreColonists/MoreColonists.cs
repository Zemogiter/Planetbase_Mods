using Planetbase;
using System;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine.UI;

namespace MoreColonists
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Extra colonists (number is double for bigger ships)")] public int moreColonist = 5;
        [Draw("As above but for visitors")] public int visitors = 10;
        [Draw("Random chance for colonist ships to contain bots")] public bool botColonistsMode = true;
        [Draw("Dont increase intruder numbers?")] public bool noIntruders = true;
        [Draw("Can visitors carry flu?")] public bool canBeCarrier = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class MoreColonists : ModBase
	{
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new MoreColonists(), modEntry, "MoreColonists");
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
		{
			
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
            if (MoreColonists.settings.visitors != 0)
            {
                num = MoreColonists.settings.visitors;
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
                if (Random.Range(0, 20) == 0 && MoreColonists.settings.canBeCarrier)
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
            int num = MoreColonists.settings.moreColonist;
            if (value > 0.9f)
            {
                num += Random.Range(4, 7);
            }
            else if (value > 0.7f)
            {
                num += Random.Range(2, 5);
            }
            if (__instance.mSize == Size.Large)
            {
                num *= 2;
            }
            if (__instance.mIntruders)
            {
                if (MoreColonists.settings.noIntruders == true)
                {
                    num += 0;
                }
                else
                {
                    num += LandingShipManager.getExtraIntruders();
                }
            }
            
            for (int i = 0; i < num; i++)
            {
                MethodInfo getMethod = __instance.GetType().GetMethod("calculateSpecialization", BindingFlags.NonPublic | BindingFlags.Instance);
                var calculation = getMethod.Invoke(__instance, new object[] {});
                Specialization specialization = (Specialization)((!__instance.mIntruders) ? calculation : TypeList<Specialization, SpecializationList>.find<Intruder>());
                if (specialization != null)
                {
                    Character.create(specialization, __instance.getSpawnPosition(i), Location.Exterior);
                }
            }
            if(MoreColonists.settings.botColonistsMode == true && Random.Range(0, 20) == 0)
            {
                for (int j = 0; j < Mathf.RoundToInt(Random.value); j++)
                {
                    Specialization specializationCarrier = TypeList<Specialization, SpecializationList>.find<Carrier>();
                    Specialization specializationConstructor = TypeList<Specialization, SpecializationList>.find<Constructor>();
                    Specialization specializationDriller = TypeList<Specialization, SpecializationList>.find<Driller>();
                    Character.create(specializationCarrier, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                    Character.create(specializationConstructor, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                    Character.create(specializationDriller, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                }
            }
        }
    }
}
