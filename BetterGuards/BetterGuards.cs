using System.Linq;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace BetterGuards
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Enable gun drops?")] public bool EnableGunDrops = true;
        [Draw("Intruders have better guns? (if true intruders will drop guns with higher durability)")] public bool IntruderGunHighDurability = false;
        [Draw("Health multiplier for guards")] public float Healthmult = 2f;
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
    [HarmonyPatch(typeof(Character), nameof(Character.create), new[] { typeof(Specialization), typeof(Vector3), typeof(Location) })]
    public class GuardPatch
    {
        //replacing the default health bar with a larger one for guards
        public static void Postfix(Character __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>())
            {
                var indicators = __instance.getIndicators();
                Indicator indicator = (Indicator)indicators.Where(indicator => indicator.getSignType() == SignType.Health);
                //Debug.Log("BetterGuards - The indicator before the changes is " + indicator.getLevel() + indicator.getName());
                indicator.setMax(indicator.getMax() * BetterGuards.settings.Healthmult);
                //Debug.Log("BetterGuards - New indicator's levels " + indicator.getLevel() + indicator.getName());
                indicator.setOrientation(IndicatorOrientation.Vertical);
                //indicator.setLevels();
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.onKo))]
    public class CharacterPatch
    {
        //make guards and intruders drop guns on death (vanilla = guns disappear with them)
        public static void Postfix(Character __instance)
        {
            if(BetterGuards.settings.EnableGunDrops && __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>() || __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
            {
                Vector3 position = __instance.getPosition();
                Location location = __instance.getLocation();
                ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();
                Resource droppedGun = Resource.create(gunType, position, location);
                if(BetterGuards.settings.IntruderGunHighDurability && __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
                {
                    droppedGun.setDurability(Resource.Durability.High);
                }
            }
        }
    }
}
