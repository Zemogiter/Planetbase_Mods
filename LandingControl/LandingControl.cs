using System;
using System.Collections.Generic;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace LandingControl
{
    public class LandingControl : ModBase
    {
        public GuiRenderer r;
        public GuiDefinitions.Callback panelCallback;
        public ModuleType.Panel panel;
        public static new void Init(ModEntry modEntry) => InitializeMod(new LandingControl(), modEntry, "LandingControl");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }

    }
    [HarmonyPatch(typeof(GuiInfoPanelRenderer), nameof(GuiInfoPanelRenderer.onPanelCallback))]
    public class GuiInfoPanelRendererPatch : GuiInfoPanelRenderer
    {
        public GuiInfoPanelRendererPatch(GuiRenderer guiRenderer) : base(guiRenderer)
        {
        }

        public static bool Prefix(GuiDefinitions.Callback panelCallback, ModuleType.Panel panel)
        {
            switch (panel)
            {
                case ModuleType.Panel.LandingPermissions:
                    panelCallback(new GuiLandingPermissionsPatch());
                    break;
                case ModuleType.Panel.SecurityControls:
                    panelCallback(new GuiSecurityWindow());
                    break;
                case ModuleType.Panel.ManufacturingLimits:
                    panelCallback(new GuiManufactureLimitsWindow());
                    break;
            }

            return false;
        }
    }
    [HarmonyPatch(typeof(GuiLandingPermissions), nameof(GuiLandingPermissions.onReset))]
    public class GuiLandingPermissionsPatch
    {
        public static bool Prefix()
        {
            var specializationAmount = CoreUtils.GetMember<GuiLandingPermissions, List<GuiAmountSelector>>("mSpecializationAmountSelectors");
            var steps = CoreUtils.GetMember<GuiAmountSelector, int>("mStep");
            var changeCallback = CoreUtils.GetMember<GuiAmountSelector, GuiDefinitions.Callback>("mChangeCallback");
            var flags = CoreUtils.GetMember<GuiAmountSelector, int>("mFlags");
            var tooltip = CoreUtils.GetMember<GuiAmountSelector, string>("mStep");
            var callback = CoreUtils.GetMember<GuiLandingPermissions, GuiDefinitions.Callback>("mCallback");
            foreach (GuiAmountSelector selector in specializationAmount)
            {
                steps = 1;
                changeCallback = null;
                flags = 0;
                tooltip = null;

                //callback = new GuiDefinitions.Callback(GuiLandingPermissions.onReset);
            }

            LandingPermissions landingPermissions = LandingShipManager.getInstance().getLandingPermissions();
            foreach (Specialization specialization in SpecializationList.getColonistSpecializations())
            {
                landingPermissions.getSpecializationPercentage(specialization).set(0);
            }
            return true;
        }
    }
}
