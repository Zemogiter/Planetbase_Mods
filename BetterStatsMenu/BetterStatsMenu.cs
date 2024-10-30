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

        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("tooltip_manufacture_limit_F2", Message);
        }
    }
    [HarmonyPatch(typeof(GuiStatsWindow), nameof(GuiStatsWindow.addSpecializationItem))]
    public class BotDisplayPatch
    {
        public static bool Prefix(Specialization specialization, GuiWindowItem parentItem)
        {
            int botLimitsCarrier = Singleton<ManufactureLimits>.getInstance().getBotLimit(TypeList<Specialization, SpecializationList>.find<Carrier>()).get();
            int botLimitsConstructor = Singleton<ManufactureLimits>.getInstance().getBotLimit(TypeList<Specialization, SpecializationList>.find<Constructor>()).get();
            int botLimitsDriller = Singleton<ManufactureLimits>.getInstance().getBotLimit(TypeList<Specialization, SpecializationList>.find<Driller>()).get();
            string text = Character.getCountOfSpecialization(specialization).ToString();
            string text2 = specialization.getNamePlural();
            if (armedCount > 0)
            {
                string text3 = text;
                text = text3 + " (" + armedCount + ")";
                text3 = text2;
                text2 = text3 + " (" + StringList.get("tooltip_armed") + ": " + armedCount + ")";
            }
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
    /*[HarmonyPatch(typeof(GuiStatsWindow), nameof(GuiStatsWindow.updateUi))]
    public class ResourceDisplayPatch
    {
        public static void Postfix(GuiStatsWindow __instance)
        {
            foreach (ResourceType item in TypeList<ResourceType, ResourceTypeList>.get())
            {
                int amount = totalAmounts.getAmount(item);
                int amount2 = resourceAmounts.getAmount(item);
                string text2 = amount2.ToString();
                string tooltip = item.getName() + ": " + amount;
                if (amount != amount2 && item.hasFlag(128))
                {
                    string text3 = text2;
                    text2 = text3 + " (" + amount + ")";
                    tooltip = StringList.get("tooltip_resource_usable", item.getName(), amount2.ToString(), amount.ToString());
                }
                guiRowItem.addChild(new GuiLabelItem(text2, item.getIcon(), tooltip));
                num++;
                if (num >= num2)
                {
                    guiRowItem = new GuiRowItem(num2);
                    guiSectionItem4.addChild(guiRowItem);
                    num = 0;
                }
            }
        }
    }*/
}
