using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionMethods
{
    public static class StringExtensions
    {
        public static string getTimeScaleCountString(this GuiGeneralPanel __instance)
        {
            return TimeSpeedDisplay.GuiGeneralPanelPatch.mTimeScaleCountString;
        }
    }
}
