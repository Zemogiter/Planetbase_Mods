using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
            var renderer = new CustomGuiInfoPanelRenderer(r);
            renderer.onPanelCallback(panelCallback, panel); ;

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }

    }
    public class CustomGuiInfoPanelRenderer : GuiInfoPanelRenderer
    {
        public CustomGuiInfoPanelRenderer(GuiRenderer r) : base(r) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Original source hode has it starting with lowercase letter.")]
        public void onPanelCallback(GuiDefinitions.Callback panelCallback, ModuleType.Panel panel)
        {
            switch (panel)
            {
                case ModuleType.Panel.LandingPermissions:
                    panelCallback(new CustomGuiLandingPermissions());
                    break;
                case ModuleType.Panel.SecurityControls:
                    panelCallback(new GuiSecurityWindow());
                    break;
                case ModuleType.Panel.ManufacturingLimits:
                    panelCallback(new GuiManufactureLimitsWindow());
                    break;
            }
        }
    }
}
