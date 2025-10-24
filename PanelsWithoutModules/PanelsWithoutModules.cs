using System.Linq;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace PanelsWithoutModules
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Enable Manufacturing Limits panel without Factory?")] public bool NoFactoryNeeded = true;
        [Draw("Enable Security Console without Control Center?")] public bool NoControlCenterNeeded = true;
        [Draw("Enable Landing Permissions panel without Landing Pad?")] public bool NoLandingPadNeeded = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class PanelsWithoutModules : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new PanelsWithoutModules(), modEntry, "PanelsWithoutModules");
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
            //nothing required here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing required here
        }
    }
    [HarmonyPatch(typeof(GuiMenuSystem), "init")]
    public class BaseManagementMenuPatch
    {
        public static void Postfix(ref GuiMenuSystem __instance, GameStateGame gameStateGame)
        {
            GuiMenu menu = __instance.GetMenu("mMenuBaseManagement");
            if (menu != null)
            {
                ModuleType moduleType = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>();
                ModuleType moduleType1 = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeControlCenter>();
                ModuleType moduleType2 = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeLandingPad>();

                GuiMenuItem guiMenuItem = menu.getItems().FirstOrDefault((GuiMenuItem x) => x.getRequiredModuleType() == moduleType);
                GuiMenuItem guiMenuItem1 = menu.getItems().FirstOrDefault((GuiMenuItem y) => y.getRequiredModuleType() == moduleType1);
                GuiMenuItem guiMenuItem2 = menu.getItems().FirstOrDefault((GuiMenuItem z) => z.getRequiredModuleType() == moduleType2);

                if (guiMenuItem != null && PanelsWithoutModules.settings.NoFactoryNeeded)
                {
                    guiMenuItem.SetCallback(gameStateGame.toggleWindow<GuiManufactureLimitsWindow>);
                    guiMenuItem.setRequiredModuleType(null);
                }
                if (guiMenuItem1 != null && PanelsWithoutModules.settings.NoControlCenterNeeded)
                {
                    guiMenuItem1.SetCallback(gameStateGame.toggleWindow<GuiSecurityWindow>);
                    guiMenuItem1.setRequiredModuleType(null);
                }
                if (guiMenuItem2 != null && PanelsWithoutModules.settings.NoLandingPadNeeded)
                {
                    guiMenuItem2.SetCallback(gameStateGame.toggleWindow<GuiLandingPermissions>);
                    guiMenuItem2.setRequiredModuleType(null);
                }
            }
        }
    }
}
