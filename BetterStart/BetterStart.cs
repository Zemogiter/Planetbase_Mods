using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using UnityModManagerNet;
using System.Reflection;
using UnityEngine;

namespace BetterStart
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Add extra resources to the Colony Ship for each planet type in the New Game menu (currently dosen't touch challenges)")] public bool addExtraResources = true;
        [Draw("Number of resources to add. Separate for every resource (i.e. 10 Metal, 10 Bioplastic etc)")] public int extraResourceCount = 15;
        [Draw("Add extra bots to the Colony Ship for each planet type in the New Game menu (currently dosen't touch challenges)")] public bool addExtraBots = true;
        [Draw("Number of bots to add. Separate for every bot type (i.e. 5 Carriers, 5 Drillers etc)")] public int extraBotCount = 5;
        [Draw("Unlock more modules to be available for construction right after landing. Affects challenges as well.")] public bool unlockMoreAtStart = true;
        [Draw("Spawn ruins of previous settlement in an area around your landing position. Currently untested.")] public bool spawnRuins = false;
        [Draw("Distance from your landing position in which ruins will spawn.")] public float ruinsSpawnDistance = 500f;
        [Draw("Debug mode")] public bool debugMode = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class BetterStart : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new BetterStart(), modEntry, "BetterStart");
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
            var typesToChange = new[]
            {
                typeof(PlanetClassD),
                typeof(PlanetClassF),
                typeof(PlanetClassM),
                typeof(PlanetClassS),
            };
            if (BetterStart.settings.addExtraResources)
            {
                // adds extra resources to starting pool for each of the 4 main planet
                
                foreach (Type t in typesToChange)
                {
                    Planet currPlanet = PlanetList.find(t.Name);
                    currPlanet.getStartingResources().add(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), BetterStart.settings.extraResourceCount);
                    currPlanet.getStartingResources().add(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), BetterStart.settings.extraResourceCount);
                    currPlanet.getStartingResources().add(TypeList<ResourceType, ResourceTypeList>.find<MedicalSupplies>(), BetterStart.settings.extraResourceCount);
                    currPlanet.getStartingResources().add(TypeList<ResourceType, ResourceTypeList>.find<Spares>(), BetterStart.settings.extraResourceCount);

                    if (BetterStart.settings.debugMode)
                    {
                        foreach (var resource in currPlanet.getStartingResources())
                        {
                            Console.WriteLine("BetterStart - outputing the moddified starting resources list: " + resource.ToString() + " going to " + t.Name);
                        }
                    }
                }
            }
            if (BetterStart.settings.addExtraBots) //need to check if this works
            {
                foreach(Type t in typesToChange)
                {
                    Planet currPlanet = PlanetList.find(t.Name);
                    currPlanet.getStartingSpecializations().Add(new SpecializationCount(TypeList<Specialization, SpecializationList>.find<Carrier>(), BetterStart.settings.extraBotCount));
                    currPlanet.getStartingSpecializations().Add(new SpecializationCount(TypeList<Specialization, SpecializationList>.find<Constructor>(), BetterStart.settings.extraBotCount));
                    currPlanet.getStartingSpecializations().Add(new SpecializationCount(TypeList<Specialization, SpecializationList>.find<Driller>(), BetterStart.settings.extraBotCount));

                    if (BetterStart.settings.debugMode)
                    {
                        foreach (var specialization in currPlanet.getStartingSpecializations())
                        {
                            Console.WriteLine("BetterStart - outputing the moddified starting specialization list: " + specialization.getSpecialization().getName() + " , number of: " + specialization.getCount() + " going to " + t.Name);
                        }
                    }
                }
            }
            if (BetterStart.settings.unlockMoreAtStart)
            {
                //unlocks some modules to be available for construction right after landing
                Type[] typesToEnable = new[] 
                {
                    // Internal modules
                    typeof(ModuleTypeStorage),
                    typeof(ModuleTypeCanteen),
                    typeof(ModuleTypeMultiDome),
                    typeof(ModuleTypeBioDome),
                    typeof(ModuleTypeProcessingPlant),
                    typeof(ModuleTypeControlCenter),
                    
                    // External modules
                    typeof(ModuleTypeWindTurbine),
                    typeof(ModuleTypeLandingPad),
                    typeof(ModuleTypeWaterTank),
                    typeof(ModuleTypeSignpost),
                };
                foreach (Type t in typesToEnable)
                {
                    FieldInfo mReqField = t
                        .GetField("mRequiredStructure", BindingFlags.NonPublic | BindingFlags.Instance);
                    ModuleType moduleInst = ModuleTypeList.find(t.Name);

                    mReqField?.SetValue(moduleInst, new ModuleTypeRef());
                }
            }
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
        public override void OnGameStart(GameStateGame gameStateGame)
        {
            if (CameraManager.getInstance().isCinematic() == true && ColonyShip.getFirstOfType<ColonyShip>() != null && BetterStart.settings.spawnRuins == true && ChallengeManager.getInstance().isChallengeEnabled() == false) //checks if we are in the game, we're starting a new save, ruins are enabled and we're not in a challenge
            {
                System.Random random = new System.Random();
                int numberOfBuildings = random.Next(1, 10); // Random number of buildings to spawn

                for (int i = 0; i < numberOfBuildings; i++)
                {
                    float angle = (float)(random.NextDouble() * Math.PI * 2);
                    float distance = (float)(random.NextDouble() * BetterStart.settings.ruinsSpawnDistance);
                    float x = distance * Mathf.Cos(angle);
                    float z = distance * Mathf.Sin(angle);

                    Vector3 spawnPosition = new Vector3(x, 0, z) + ColonyShip.getAveragePosition();
                    SpawnRandomBuilding(spawnPosition);
                }
            }
        }

        private void SpawnRandomBuilding(Vector3 position)
        {
            System.Random random = new System.Random();
            Type[] buildingTypes = new[]
            {
                typeof(ModuleTypeStorage),
                typeof(ModuleTypeCanteen),
                typeof(ModuleTypeMultiDome),
                typeof(ModuleTypeBioDome),
                typeof(ModuleTypeProcessingPlant),
                typeof(ModuleTypeControlCenter),
                typeof(ModuleTypeWindTurbine),
                typeof(ModuleTypeLandingPad),
                typeof(ModuleTypeWaterTank),
                typeof(ModuleTypeSignpost),
            };

            Type buildingType = buildingTypes[random.Next(buildingTypes.Length)];
            ModuleType building = ModuleTypeList.find(buildingType.Name);
            Planetbase.Module.create(position,1,building).debugDamage(random.range(0f,1f));
        }
    }
}
