using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Reflection;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace PatientTraders
{
    [DrawFields(DrawFieldMask.Public)]
    public class TimesSettings
    {
        public static float newStayTime = 560f;
        public static float newTradeTime = 6800f;
    }    
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Change staying time for the merchants? Game default is 180 seconds.")] public bool changeStayTime = true;
        [Draw("Change trading time?")] public bool changeTradeTime = true;
        [Draw("Add extra bots to the merchant ships?")] public bool addBots = true;
        [Draw("Debug mode")] public bool debugMode = false; 
        [Draw("Settings", Collapsible = true)] public TimesSettings TimesSettings = new();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }
    public class PatientTraders : ModBase
    {

        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new PatientTraders(), modEntry, "PatientTraders");
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
            //MethodLister.ListMethods();
            /*if (PatientTraders.settings.debugMode == true)
            {
                var stayTime = CoreUtils.GetMember<MerchantShip, float>("StayTime");
                Console.WriteLine("PatientTraders - vanilla stay time is 180, current one is: " + stayTime.ToString());
                var tradeTime = CoreUtils.GetMember<MerchantShip, float>("TradeTime");
                Console.WriteLine("PatientTraders - vanilla trade time is 1200, current one is: " + tradeTime.ToString());
            }*/
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    public class MethodLister
    {
        public static void ListMethods()
        {
            Type type = typeof(MerchantShip);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (MethodInfo method in methods)
            {
                Console.WriteLine($"Method: {method.Name}, IsPrivate: {method.IsPrivate}, IsPublic: {method.IsPublic}");
            }
        }
    }
    
    //adding extra bots to the merchant ships
    [HarmonyPatch(typeof(MerchantShip), "addBotProducts")]
    public class TraderBotPatch
    {
        public static void Postfix(MerchantShip __instance)
        {
            if (PatientTraders.settings.addBots == false)
            {
                return;
            }
            if (__instance.getCategory() == MerchantCategory.Industrial)
            {
                var botProducts = CoreUtils.GetMember<MerchantShip, ProductAmounts>("mProducts", __instance);
                botProducts.add(new ProductBot(TypeList<Specialization, SpecializationList>.find<Carrier>()), 3);
                botProducts.add(new ProductBot(TypeList<Specialization, SpecializationList>.find<Driller>()), 1);
            }
            if (__instance.getCategory() == MerchantCategory.RawMaterial)
            {
                var botProducts = CoreUtils.GetMember<MerchantShip, ProductAmounts>("mProducts", __instance);
                botProducts.add(new ProductBot(TypeList<Specialization, SpecializationList>.find<Carrier>()), 1);
                botProducts.add(new ProductBot(TypeList<Specialization, SpecializationList>.find<Driller>()), 3);
            }
        }
    }
    
}
