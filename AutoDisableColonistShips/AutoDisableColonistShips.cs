using Planetbase;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;

namespace AutoDisableColonistShips
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Trigger Value (if max oxygen genration - oxygen usage is equal or less than this number, mod will trigger)")] public int TriggerValue = 4;
        [Draw("Re-enable ships once you go above trigger value?")] public bool ReEnableShips = false;
        [Draw("Disallow visitor ships as well?")] public bool DisallowVisitorShips = false;
        [Draw("Manual Override (enable colonist/visitor ships even when below trigger value)")] public bool manualOverride = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class AutoDisableColonistShips : ModBase
    {
        public static bool enabled;
        public static Settings settings;

        //these bools are needed so that we wont display the same message over and over
        public static bool messageDisplayed = false;
        public static bool messageDisplayedNormal = false;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new AutoDisableColonistShips(), modEntry, "AutoDisableColonistShips");
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
        public const string MESSAGE = "Geting low on oxygen, disallowing colonist ships from landing.";
        public const string MESSAGE2 = "Geting low on oxygen, disallowing colonist and visitor ship from landing.";
        public const string MESSAGE3 = "Oxygen levels green, colonist ships are allowed entry.";
        public const string MESSAGE4 = "Oxygen levels green, colonist and visitor ships are allowed entry.";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if(GameManager.getInstance().getGameState() is GameStateGame gameStateGame)
            {
                //getting the necessary variables
                var landingPermissions = LandingShipManager.getInstance().getLandingPermissions();
                var refBool = landingPermissions.getColonistRefBool();
                var refBoolVisitors = landingPermissions.getVisitorRefBool();

                if (settings.manualOverride == true)
                {
                    return;
                }
                //checking if we even have oxygen generators on map as well as other basic base functions covered to avoid unnecessary messages
                if (Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeOxygenGenerator>()) > 0 && Module.getOperationalCountOfType(ModuleTypeList.find<ModuleTypeWaterExtractor>()) > 0 && Module.getOverallPowerBalance() > 0)
                {
                    LowOxygenCheck(refBool, refBoolVisitors, gameStateGame);
                    HighOxygenCheck(refBool, refBoolVisitors, gameStateGame);
                }
            }
        }

        private static void RegisterStrings()
        {
            StringUtils.RegisterString("message_low_oxygen_landing_disabled", MESSAGE);
            StringUtils.RegisterString("message_low_oxygen_landing_disabled_2", MESSAGE2);
            StringUtils.RegisterString("message_oxygen_level_normal", MESSAGE3);
            StringUtils.RegisterString("message_oxygen_level_normal_2", MESSAGE4);
        }
        private void LowOxygenCheck(RefBool refBool, RefBool refBoolVisitors, GameStateGame gameStateGame)
        {
            //without visitor ships
            if (refBool.get() == true && settings.DisallowVisitorShips == false && CountOxygenUsers() <= settings.TriggerValue)
            {
                refBool.set(false);
                if (messageDisplayed == false)
                {
                    //CoreUtils.InvokeMethod("addToast", gameStateGame, [MESSAGE, 3f]);
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled", MESSAGE), ResourceList.StaticIcons.Oxygen, 1));
                    messageDisplayed = true;
                    messageDisplayedNormal = false;
                }
            }
            //with vistior ships
            else if (refBool.get() == true && settings.DisallowVisitorShips == true && CountOxygenUsers() <= settings.TriggerValue)
            {
                refBool.set(false);
                refBoolVisitors.set(false);
                if (messageDisplayed == false)
                {
                    //CoreUtils.InvokeMethod("addToast", gameStateGame, [MESSAGE2, 3f]);
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_oxygen_landing_disabled_2", MESSAGE2), ResourceList.StaticIcons.Oxygen, 8));
                    messageDisplayed = true;
                    messageDisplayedNormal = false;
                }
            }
        }
        private void HighOxygenCheck(RefBool refBool, RefBool refBoolVisitors, GameStateGame gameStateGame)
        {
            //without visitor ships
            if (settings.ReEnableShips == true && settings.DisallowVisitorShips == false && CountOxygenUsers() >= settings.TriggerValue && messageDisplayed == true)
            {
                refBool.set(true);
                if (messageDisplayedNormal == false)
                {
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal", MESSAGE3), ResourceList.StaticIcons.Oxygen, 8));
                    messageDisplayedNormal = true;
                    messageDisplayed = false;
                }
                
            }
            //with vistior ships
            else if (settings.ReEnableShips == true && settings.DisallowVisitorShips == true && CountOxygenUsers() >= settings.TriggerValue && messageDisplayedNormal == false && messageDisplayed == true)
            {
                refBool.set(true);
                refBoolVisitors.set(true);
                if (messageDisplayedNormal == false)
                {
                    Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_level_normal_2", MESSAGE4), ResourceList.StaticIcons.Oxygen, 8));
                    messageDisplayedNormal = true;
                    messageDisplayed = false;
                }
            }
            // setting these back to false so that if the condition ever happens again during gameplay, mod will be ready to trigger again
            messageDisplayedNormal = false;
            messageDisplayed = false;
        }
        public int CountOxygenUsers() //strangely enough, this function dosen't exist in the game
        {
            int numberofColonists = Character.getHumanCount();
            int maxNumber = Module.getOverallOxygenGeneration();

            return maxNumber - numberofColonists;
        }
    }
}
