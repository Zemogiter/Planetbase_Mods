using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Reflection;
using static Planetbase.ModuleType;

namespace PanelsWithoutModules
{
    public class PanelsWithoutModules : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new PanelsWithoutModules(), modEntry, "PanelsWithoutModules");

        public override void OnInitialized(ModEntry modEntry)
        {
            ModuleName control = ModuleName.ControlCenter;
            typeof(ModuleTypeControlCenter).GetField("mRelatedPanel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(control, Panel.Invalid);
            ModuleName landing = ModuleName.LandingPad;
            typeof(ModuleTypeLandingPad).GetField("mRelatedPanel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(landing, Panel.Invalid);
            ModuleName factory = ModuleName.Factory;
            typeof(ModuleTypeFactory).GetField("mRelatedPanel", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(factory, Panel.Invalid);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing required here
        }
    }
    [HarmonyPatch(typeof(GuiMenuSystem), nameof(GuiMenuSystem.init))]
    public class BaseManagementMenuPatch
    {
        static void Postfix(GuiMenuSystem __instance)
        {
            var landing_permissions = __instance.mMenuBaseManagement.getItem(3);
            landing_permissions.setRequiredModuleType(null);
            var security_controls = __instance.mMenuBaseManagement.getItem(4);
            security_controls.setRequiredModuleType(null);
            var manufacture_limits = __instance.mMenuBaseManagement.getItem(5);
            manufacture_limits.setRequiredModuleType(null);

            /*foreach(GuiMenuItem item in __instance.mMenuBaseManagement.mItems)
            {
                item.setRequiredModuleType(null);
            }*/
        }
    }
}
