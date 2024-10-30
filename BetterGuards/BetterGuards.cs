using System;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace BetterGuards
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Enable gun drops?")] public bool EnableGunDrops = true;
        [Draw("Intruders have better guns?")] public bool IntruderGunHighDurability = false;
        [Draw("Health multiplier for guards")] public float Healthmult = 4.2f;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class BetterGuards : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public new static void Init(ModEntry modEntry)
        {
            settings = ModSettings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new BetterGuards(), modEntry, "BetterGuards");
        }
        static void OnGUI(ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.create))]
    public class GuardPatch
    {
        //replacing the default health bar with a larger one for guards
        public static void Postfix(Character instance)
        {
            if (instance == null)
            {
                return;
            }
            if (instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>())
            {
                Indicator indicator = new(StringList.get("health"), ResourceList.StaticIcons.Health, IndicatorType.Normal, 1f, BetterGuards.settings.Healthmult, SignType.Health);
                Console.WriteLine("The new indicator is " + indicator);
                indicator.setLevels(0.1f, 0.5f, 0.7f, 0.8f);
                Console.WriteLine("New indicator's levels " + indicator.getLevels());
                indicator.setOrientation(IndicatorOrientation.Vertical);
                CharacterIndicator healthIndicator = CharacterIndicator.Health;
                instance.getIndicator(healthIndicator).setValue(indicator.getMax());
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.onKo))]
    public class CharacterPatch
    {
        //make guards and intruders drop guns on death (vanilla = guns disappear with them)
        public static void Postfix(Character instance)
        {
            if(BetterGuards.settings.EnableGunDrops && instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>() || instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
            {
                Vector3 position = instance.getPosition();
                Location location = instance.getLocation();
                ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();
                Resource droppedGun = Resource.create(gunType, position, location);
                if(BetterGuards.settings.IntruderGunHighDurability && instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
                {
                    droppedGun.setDurability(Resource.Durability.High);
                }
            }
        }
    }
}
