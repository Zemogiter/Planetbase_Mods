using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityModManagerNet;
using UnityEngine.UI;
using HarmonyLib;

namespace GameplayConstantChanger
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Max working time")] public const float MaxWorkingTime = 65f;
        [Draw("Merchant Ship Period")]public const float MerchantShipPeriod = 540f;
        [Draw("Colonist Ship Period")] public const float ColonistShipPeriod = 240f;
        [Draw("Visitor Ship Period")] public const float VisitorShipPeriod = 300f;
        [Draw("Mining time")] public const float MiningTime = 150f;
        [Draw("Minimum mood for colonist landing")] public const float MinStatusForColonistLanding = 0.5f;
        [Draw("Minimum prestige for visitors")] public const float MinPrestigeForVisitorLanding = 0.075f;
        [Draw("Vegetable growing time")] public const float VegetableProductionTime = 540f;
        [Draw("Metal prodiction time")] public const float MetalProductionTime = 120f;
        [Draw("Bioplastic production time")] public const float BioplasticProductionTime = 100f;
        [Draw("Bot assembly time")] public const float BotProductionTime = 420f;
        [Draw("Medical supplies production time")] public const float MedicalSuppliesProductionTime = 180f;
        [Draw("Meat growth time")] public const float VitromeatProductionTime = 210f;
        [Draw("Spares production time")] public const float SparesProductionTime = 120f;
        [Draw("Gun production time")] public const float GunProductionTime = 300f;
        [Draw("Chip production time")] public const float SemiconductorProductionTime = 300f;
        [Draw("Tree growth time")] public const float TreeGrowthTime = 120f;
        [Draw("Rage decision time")] public const float RageDecisionTime = 30f;
        [Draw("Firearm range")] public const float FightRange = 1.5f;
        [Draw("Max fight time")] public const float MaxFightTime = 20f;
        [Draw("Fight health decay time")] public const float FightHealthDecayTime = 120f;
        [Draw("Storm speed factor")] public const float StormSpeedFactor = 0.25f;
        [Draw("Storm Health decay time")] public const float StormHealthDecayTime = 240f;
        [Draw("Solar flate health decay time")] public const float SolarFlareHealthDecayTime = 30f;
        [Draw("Storm condition decay time (for bots)")] public const float StormConditionDecayTime = 600f;
        [Draw("Solar flate condition decay time (for bots)")] public const float SolarFlareConditionDecayTime = 180f;
        [Draw("Meal food recovery")] public const float MealFoodRecovery = 0.4f;
        [Draw("Morale recovery from varied meal")] public const float VariedMealMoraleRecovery = 0.025f;
        [Draw("Meal consumption time")] public const float MealConsumptionTime = 30f;
        [Draw("Morale recovery from alcohol")] public const float AlcoholicDrinkMoraleRecovery = 0.3f;
        [Draw("Drinking time")] public const float DrinkConsumptionTime = 90f;
        [Draw("Grace period for indicator decay")] public const float IndicatorDecayGracePeriod = 120f;
        [Draw("Nutrition decay time")] public const float NutritionDecayTime = 1200f;
        [Draw("Hydration decay time")] public const float HydrationDecayTime = 1200f;
        [Draw("Sleep decay time")] public const float SleepDecayTime = 1800f;
        [Draw("Sleep restore time")] public const float SleepRestoreTime = 300f;
        [Draw("Repair time (for building that use spares)")] public const float BuildableRepairTime = 60f;
        [Draw("Food component repair time")] public const float FoodComponentRepairTime = 180f;
        [Draw("Condition decay time for solar")] public const float SolarPanelConditionDecayTime = 2400f;
        [Draw("Condition decay time for wind")] public const float WindTurbineConditionDecayTime = 3300f;
        [Draw("Condition decay time for vegetable pads")] public const float VegetablePadConditionDecayTime = 720f;
        [Draw("Condition decay time for tissue synthesizer")] public const float TissueSynthesizerConditionDecayTime = 720f;
        [Draw("Decay time for health after getting KO'd")] public const float HealthKoDecayTime = 20f;
        [Draw("Bot condition decay time")] public const float BotConditionDecayTime = 480f;
        [Draw("Grace period for condition decay")] public const float ConditionDecayGracePeriod = 2400f;
        [Draw("Grace period for injuries from mining")] public const float MiningTraumaGracePeriod = 1800f;
        [Draw("Mining accident period")] public const float MiningAccidentPeriod = 30f;
        [Draw("Chance to receive mining injuries")] public const int MiningTraumaChance = 40;
        [Draw("Contagion Period")] public const float ContagionPeriod = 30f;
        [Draw("Chance for colonists to catch a infection")] public const int ContagionChance = 3;
        [Draw("Contagion spread distance(any character in this circle has a chance to become infected)")] public const float ContagionDistance = 10f;
        [Draw("Squared contagion distance")] public const float SqrContagionDistance = 100f;
        [Draw("Basic meal malnutrition count")] public const int BasicMealMalnutritionCount = 15;
        [Draw("Health recovery time")] public const float HealthRecoverTime = 300f;
        [Draw("Hydration recovery time")] public const float DrinkingFountainWaterRecoveryTime = 20f;
        [Draw("Restoration time(?)")] public const float RestoreTime = 50f;
        [Draw("Morale decay time from working")] public const float WorkMoraleDecayTime = 720f;
        [Draw("Morale decay time from carrying")] public const float CarryMoraleDecayTime = 720f;
        [Draw("Distance from dying colonist that will affect other's morale")] public const float DeathDecayMoraleRadius = 10f;
        [Draw("Amount of morale taken away from nearby deaths")] public const float DeathDecayMoraleAmount = 0.2f;
        [Draw("Morale restoration time from beds")] public const float BedMoraleRestoreTime = 480f;
        [Draw("Default morale restore time")] public const float DefaultMoraleRestoreTime = 360f;
        [Draw("Oxygen decay time (outside)")] public const float HumanOxygenExteriorDecayTime = 480f;
        [Draw("Oxygen restoration time (inside)")] public const float HumanOxygenInteriorRestoreTime = 20f;
        [Draw("Oxygen decay time (inside)")] public const float HumanOxygenInteriorDecayTime = 20f;
        [Draw("Oxygen consumption per one human")] public const int HumanOxygenConsumption = 1;
        [Draw("How fasts humans walk")] public const float HumanWalkingSpeed = 4f;
        [Draw("How fast bots walk")] public const float BotWalkingSpeed = 6f;
        [Draw("Terrain size")] public const float TerrainSize = 2000f;
        [Draw("Game area size")] public const float GameAreaSize = 750f;
        [Draw("Period between intruder attacks")] public const float IntruderPeriod = 4200f;
        [Draw("Randomness of above")] public const float IntruderPeriodVariance = 1800f;
        [Draw("Intruder aggression time")] public const float IntruderAggressionTime = 60f;
        [Draw("Intruder leave time")] public const float IntruderLeaveTime = 120f;
        [Draw("Minimal lifetime of a bot")] public const float BotLifetimeMin = 9600f;
        [Draw("Maximal lifetime of a bot")] public const float BotLifetimeMax = 19200f;
        [Draw("Decay time of resources outside storage (for regular resources, not thoes from colony landers)")] public const float ResourceDecayTime = 3600f;
        [Draw("Slow Colonist Population")] public const int SlowColonistPopulation = 300;
        [Draw("Population slow factor")] public const float PopulationSlowFactor = 2f;
        [Draw("Build speed")] public const float BuildSpeed = 0.4f;
        [Draw("Limit of modules on map")] public const int ModuleLimit = 500;
        [Draw("How much coins per visitor")] public const int VisitorBaseFee = 5;
        [Draw("Max airlock interractions")] public const int MaxAirlockInteractions = 5;
        [Draw("Gun range")] public const float GunRange = 10f;
        [Draw("Event period")] public const float EventPeriod = 900f;
        [Draw("Disaster Warning time 1")] public const float DisasterWarningTime1 = 300f;
        [Draw("Disaster Warning time 2")] public const float DisasterWarningTime2 = 120f;
        [Draw("Minimal count of visitors")] public const int MinEventVisitorCount = 5;
        [Draw("Maximal count of visitors")] public const int MaxEventVisitorCount = 15;
        [Draw("Idle wandering time")] public const float WanderTime = 10f;
        [Draw("High priority distance offset")] public const float HighPriorityDistanceOffset = 100f;
        [Draw("How much resources you get back form deconstructing modules (80% by default)")] public const float ConstructionRecycleRatio = 0.8f;
        [Draw("How offen autosave is created")] public const float AutoSaveFrequency = 900f;
        [Draw("Radius of protection circle around anti-meteor laser")] public const float LaserRadius = 180f;
        [Draw("Radius of protection circle around lightning rod")] public const float LightningRodRadius = 50f;
        [Draw("Laser depletion time")] public const float LaserDepletionTime = 4f;
        [Draw("Laser recharge time")] public const float LaserRechargeTime = 120f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class GameplayConstantChanger : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry) 
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new GameplayConstantChanger(), modEntry, "GameplayConstantChanger"); 
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
}
