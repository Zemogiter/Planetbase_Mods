using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace BetterStatsMenu
{
    public class BetterStatsMenu : ModBase
    {
        public new static void Init(ModEntry modEntry) => InitializeMod(new BetterStatsMenu(), modEntry, "BetterStatsMenu");

        public const string Message = "Manufacture limit";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing required here
            //to-do: add a method that improves the tooltips when hovering over the numbers of specializations in the F2 menu (it displays when hovering over the number but not the icon)
            //will probably need to create CustomMethod:Method for this
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("tooltip_manufacture_limit_F2", Message);
        }
    }
    //main patch, replaces the original method to display the number of ordered bots in the same way vanilla game displays the number of armed guards in F2 menu
    [HarmonyPatch(typeof(GuiStatsWindow), "addSpecializationItem")]
    public class BotDisplayPatch
    {
        public static bool Prefix(Specialization specialization, GuiWindowItem parentItem)
        {
            // getting the numbers of ordered bots to display them next to the number of currently online bots
            int botLimitsCarrier = Singleton<ManufactureLimits>.getInstance().getBotLimit(TypeList<Specialization, SpecializationList>.find<Carrier>()).get();
            int botLimitsConstructor = Singleton<ManufactureLimits>.getInstance().getBotLimit(TypeList<Specialization, SpecializationList>.find<Constructor>()).get();
            int botLimitsDriller = Singleton<ManufactureLimits>.getInstance().getBotLimit(TypeList<Specialization, SpecializationList>.find<Driller>()).get();

            // part of the original method
            int armedCount = Character.getArmedCount(specialization);
            string text = Character.getCountOfSpecialization(specialization).ToString();
            string text2 = specialization.getNamePlural();

            if (armedCount > 0)
            {
                text = text + " (" + armedCount + ")";
                text2 = text2 + " (" + StringList.get("tooltip_armed") + ": " + armedCount + ")";
            }

            // adding the numbers of ordered bots to the text
            if (botLimitsCarrier > 0 && specialization == TypeList<Specialization, SpecializationList>.find<Carrier>())
            {
                string textBots = text;
                text = textBots + " (" + botLimitsCarrier + ")";
                textBots = text2;
                text2 = textBots + " (" + StringList.get("tooltip_manufacture_limit_F2", BetterStatsMenu.Message) + ": " + botLimitsCarrier + ")";
            }
            if (botLimitsConstructor > 0 && specialization == TypeList<Specialization, SpecializationList>.find<Constructor>())
            {
                string textBots = text;
                text = textBots + " (" + botLimitsConstructor + ")";
                textBots = text2;
                text2 = textBots + " (" + StringList.get("tooltip_manufacture_limit_F2", BetterStatsMenu.Message) + ": " + botLimitsConstructor + ")";
            }
            if (botLimitsDriller > 0 && specialization == TypeList<Specialization, SpecializationList>.find<Driller>())
            {
                string textBots = text;
                text = textBots + " (" + botLimitsDriller + ")";
                textBots = text2;
                text2 = textBots + " (" + StringList.get("tooltip_manufacture_limit_F2", BetterStatsMenu.Message) + ": " + botLimitsDriller + ")";
            }
            parentItem.addChild(new GuiLabelItem(text, specialization.getIcon(), text2));
            return false;
        }
    }
}
