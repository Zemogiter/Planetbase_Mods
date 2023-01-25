using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;

namespace LandingControl
{
    public class CustomGuiLandingPermissions : GuiLandingPermissions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Not sure if those assignments are really unused.")]
        public CustomGuiLandingPermissions() : base()
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

                callback = new GuiDefinitions.Callback(OnReset);
            }
        }

        public void OnReset(object parameter)
        {
            LandingPermissions landingPermissions = LandingShipManager.getInstance().getLandingPermissions();
            foreach (Specialization specialization in SpecializationList.getColonistSpecializations())
            {
                landingPermissions.getSpecializationPercentage(specialization).set(0);
            }
        }
    }
}