using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace EarthlikePlanet
{
    public class EarthlikePlanet : ModBase
    {
        public new static void Init(ModEntry modEntry) => InitializeMod(new EarthlikePlanet(), modEntry, "EarthlikePlanet");

        public const string PlanetName = "Earthlike Planet";
        public const string PlanetDifficulty = "Medium";
        public const string PlanetDescription = "A planet with a Earth-like gas composition, allowing for breathing without spacesuits. Small chance of previously encountered natural disasters.";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();  
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here for now
        }
        public void RegisterStrings()
        {
            StringUtils.RegisterString("planet5_name", PlanetName);
            StringUtils.RegisterString("planet5_difficulty", PlanetDifficulty);
            StringUtils.RegisterString("planet5_description", PlanetDescription);
        }
    }
    public class PlanetClassE : Planet
    {
        public PlanetClassE()
        {
            mName = StringList.get("planet5_name");
            mDifficultyString = StringList.get("planet5_difficulty");
            mDescription = StringList.get("planet5_description");
            //To-do: create this mDefinition, lack of it crashes the game on new game menu
            mDefinition = ResourceUtil.loadPrefab(FileUtils.getModLocation() + "Assets\\PrefabPlanetDefinition5").GetComponent<PlanetDefinition>();
            mAtmosphereDensity = Quantity.High;
            mLightAmount = Quantity.High;
            mSandstormRisk = Quantity.Low;
            mBlizzardRisk = Quantity.Low;
            mMeteorRisk = Quantity.Low;
            mThunderstormRisk = Quantity.Low;
            mMilestonesToUnlock = 14;
            addStartingSpecialization<Worker>(5);
            addStartingSpecialization<Biologist>(6);
            addStartingSpecialization<Engineer>(4);
            addStartingSpecialization<Medic>(2);
            addStartingSpecialization<Carrier>(1);
            addStartingResource<Metal>(30);
            addStartingResource<Bioplastic>(25);
            addStartingResource<Meal>(20);
            addStartingResource<Spares>(10);
            addStartingResource<MedicalSupplies>(5);
            mTexures = new string[4] { "Terrain/Planet2/flat1", "Terrain/Planet2/flat2", "Terrain/Planet2/slope", "Terrain/Planet2/foundations" };
            mLetter = 'E';
            mIntruderMinPrestige = 0.125f;
            mInitialMusicTrack = 1;
            initStrings();
        }
    }
    public class CustomPlanet : Planet
    {
        //Method to set colonist models outside of bases as the same as inside (i.e. no spacesuits)
        //To-do: test this once the mDefinition is completed
        public void OxygenRichPlanet()
        {
            if(PlanetManager.getInstance().getCurrentPlanetIndex() == 5)
            {
                var colonistList = CharacterUtils.GetAllCharacters();
                foreach(Character character in colonistList)
                {
                    if(character.getLocation() is Location.Exterior)
                    {
                        character.setModel(character.getSelectionModel()); //not sure how to force the suit-less models outside of bases, also not sure if the suit-less models have a variant for disasters (colonists covering heads with their arms)
                    }

                }
            }
        }
    }
    public class CustomCharacter : Character
    {
        //placeholders
        protected override List<string> getAnimationNames(CharacterAnimationType animationType)
        {
            throw new NotImplementedException();
        }

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

        //Method that should remove oxygen requirement outside of bases
        //To-do: test this once the mDefinition is completed
        public bool RequiresOxygen()
        { 
            if(PlanetManager.getInstance().getCurrentPlanetIndex() == 5)
            {
                return false;
            }
            return true;
        }
    }
    //makes the planet show up on new game menu
    //To-do: fix the nameof(), update() is not a valid option
    [HarmonyPatch(typeof(GameStateLocationSelection), nameof(GameStateLocationSelection.update))]
    public class GameStateLocationSelectionPatch
    {
        static bool Prefix(GameStateLocationSelection instance)
        {
            var planetE = new PlanetClassE();
            TypeList<Planet, PlanetList>.get().Add(planetE);
            return true;
        }
    }
}
