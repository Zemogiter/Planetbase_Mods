using System;
using System.Linq;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

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
            //fix for the issue that makes visitors lose their ship ownership upon save load and occupy the base, this should set it to the nearest visitor ship
            if (GameManager.getInstance().getGameState() is GameStateGame gameStateGame)
            {
                var visitorList = Character.getSpecializationCharacters(TypeList<Specialization, SpecializationList>.find<Visitor>());

                if (visitorList != null)
                {
                    foreach (Human visitor in visitorList.Cast<Human>())
                    {
                        if (visitor != null && visitor.getOwnedShip() == null && visitor.getState() != Character.State.Ko)
                        {
                            if (MoreColonists.settings.debugMode)
                            {
                                Console.WriteLine("MoreColonists - visitorList entry: " + visitor + " has no owned ship");
                            }
                            VisitorShip newShip = Ship.getFirstOfType<VisitorShip>();
                            if (newShip != null)
                            {
                                if (MoreColonists.settings.debugMode) Console.WriteLine("MoreColonists - found new ship for visitor: " + newShip.getPosition().ToString() + " " + newShip.getName());
                                visitor.setOwnedShip(newShip);
                                if (MoreColonists.settings.debugMode) Console.WriteLine("MoreColonists - new ship for visitor: " + visitor.getOwnedShip().ToString());
                            }
                        }
                    }
                }
            }
           
        }
    }
    /*
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
                        VisitorShip newShip = Ship.getFirstOfType<VisitorShip>();
                        if (newShip != null)
                        {
                            visitor.setOwnedShip(newShip);
                        }
                    }
                }
            }
        }
    }
    */
}
