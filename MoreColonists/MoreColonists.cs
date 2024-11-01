﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using Module = Planetbase.Module;
using Random = UnityEngine.Random;

namespace MoreColonists
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Extra colonists (number is double for bigger ships)")] public int moreColonist = 5;
        [Draw("As above but for visitors")] public int visitors = 5;
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
        public const string MESSAGE = "Bots arrived on the colonist ship. Count: ";
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
            //removal of glitched visitor ships
            var shipList = VisitorShip.mShips.Where(a => a.getName() is "VisitorShip");
            if (GameManager.getInstance().getGameState() is GameStateGame && Module.getBuiltCountOfType(TypeList < ModuleType, ModuleTypeList>.find<ModuleTypeLandingPad>()) > 0 || Module.getBuiltCountOfType(TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStarport>()) > 0 && shipList != null)
            {
#pragma warning disable IDE0220 // Add explicit cast
                foreach (VisitorShip ship in shipList)
                {
                    if (ship.mState is VisitorShip.State.Landed && ship.getPendingVisitorCount() <= 0)
                    {
                        ship.onClosingDoor();
                        ship.setState(LandingShip.State.TakingOff);
                        ship.destroy();
                    }
                }
#pragma warning restore IDE0220 // Add explicit cast
            }
		}
        public void RegisterStrings()
        {
            StringUtils.RegisterString("message_bot_colonists", MESSAGE);
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.update))]
    public class VisitorPatch
    {
        //fix for the issue that makes visitors lose their ship ownership upon save load and occupy the base, this should set the ownership to the nearest visitor ship
        //and from there the regular game code should work
        static void Postfix()
        {
            var visitorList = Character.getSpecializationCharacters(TypeList<Specialization, SpecializationList>.find<Visitor>());
            if (visitorList != null)
            {
                foreach (Human visitor in visitorList.Cast<Human>())
                {
                    if (visitor.getOwnedShip() == null && visitor.getState() != Character.State.Ko)
                    {
                        VisitorShip newShip = (VisitorShip)Ship.getFirstOfType<Ship>();
                        visitor.setOwnedShip(newShip);
                    }
                }
            }
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
                var calculation = getMethod.Invoke(__instance, new object[] {});
                Specialization specialization = (Specialization)((!__instance.mIntruders) ? calculation : TypeList<Specialization, SpecializationList>.find<Intruder>());
                if (specialization != null)
                {
                    Character.create(specialization, __instance.getSpawnPosition(i), Location.Exterior);
                }
            }
            if(MoreColonists.settings.botColonistsMode == true && Random.Range(0, 20) == 0 || MoreColonists.settings.debugMode == true)
            {
                int max = Mathf.RoundToInt(Random.value);
                for (int j = 0; j < max; j++)
                {
                    Specialization specializationCarrier = TypeList<Specialization, SpecializationList>.find<Carrier>();
                    Specialization specializationConstructor = TypeList<Specialization, SpecializationList>.find<Constructor>();
                    Specialization specializationDriller = TypeList<Specialization, SpecializationList>.find<Driller>();
                    Character.create(specializationCarrier, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                    Character.create(specializationConstructor, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                    Character.create(specializationDriller, CoreUtils.InvokeMethod<LandingShip, Vector3>("getSpawnPosition", __instance, j), Location.Exterior);
                }
                if (MoreColonists.settings.displayBotColonist == true)
                {
                    Message message = new(StringList.get("message_water_transaction", MoreColonists.MESSAGE + max), ResourceList.StaticIcons.Bot, 8);
                    Singleton<MessageLog>.getInstance().addMessage(message);
                }
            }
        }
    }
}
