﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using Random = UnityEngine.Random;

namespace MoreColonists
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Extra colonists (number is double for bigger ships)")] public int moreColonist = 5;
        [Draw("As above but for visitors")] public int moreVisitors = 5;
        [Draw("Random chance for colonist ships to contain bots(same as visitors having flu)")] public bool botColonistsMode = true;
        [Draw("Display a message if colonist ship contain bots")] public bool displayBotColonist = true;
        [Draw("Enable to decrease number of intruders on board of visitor/colonist ships")] public bool noIntruders = true;
        [Draw("Can visitors carry flu?")] public bool canBeCarrier = true;
        [Draw("Debug mode")] public bool debugMode = false;
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
			//nothing for now
        }
		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            //nothing for now
        }
    }
    [HarmonyPatch(typeof(LandingShip), nameof(LandingShip.update))]
    public class VisitorShipFixPatch
    {
        //fix for the issue that makes visitor ships not fly off once the counter reaches zero
        public static void Postfix(VisitorShip __instance)
        {
            if (__instance != null && __instance.isLanded() && __instance.getName() == "VisitorShip" && (__instance.getPendingVisitorCount() <= 0 || __instance.getDescription() == null))
            {
                Console.WriteLine("MoreColonists - state of the glitched VisitorShip: " + CoreUtils.GetMember<LandingShip, LandingShip.State>("mState", __instance) + " and the name (should be VisitorShip) is: " + __instance.getName());
                CoreUtils.InvokeMethod<LandingShip>("setState", __instance, LandingShip.State.ClosingDoor);
                CoreUtils.InvokeMethod<LandingShip>("setState", __instance, LandingShip.State.TakingOff);
            }
        }
    }
    [HarmonyPatch(typeof(Human), nameof(Human.update))]
    public class VisitorPatch
    {
        //fix for the issue that makes visitors lose their ship ownership upon save load and occupy the base, this should set it to the nearest visitor ship
        static void Postfix(Human __instance)
        {
            var visitorList = Character.getSpecializationCharacters(TypeList<Specialization, SpecializationList>.find<Visitor>());

            if (visitorList != null)
            {
                if (MoreColonists.settings.debugMode)
                {
                    foreach (var visitor in visitorList)
                    {
                        Console.WriteLine("MoreColonists - visitorList entry: " + visitor);
                    }
                }
                foreach (Human visitor in visitorList.Cast<Human>())
                {
                    if (visitor != null && visitor.getOwnedShip() == null && visitor.getState() != Character.State.Ko)
                    {
                        VisitorShip newShip = Ship.getFirstOfType<VisitorShip>() as VisitorShip;
                        if (newShip != null)
                        {
                            visitor.setOwnedShip(newShip);
                        }
                    }
                }
            }
        }
    }
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
            if (CoreUtils.GetMember<LandingShip,Size>("mSize",__instance) == Size.Large)
            {
                num *= 2;
            }
            if (CoreUtils.GetMember<LandingShip,bool>("mIntruders",__instance))
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
            if(MoreColonists.settings.botColonistsMode == true && Random.Range(0, 20) == 0) //handling extra bots in ColonistShip
            {
                int max = Mathf.RoundToInt(Random.Range(1,20));
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
