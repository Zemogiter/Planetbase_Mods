using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;

namespace PatientTraders
{
    //to-do: figure out how to display up to date, remaining newTradeTime under the pending resources for the MerchantShip
    public class CustomMerchantShip : MerchantShip
    {
        public override string getDescription()
        {
            string text = base.getDescription();
            if (Singleton<DebugManager>.getInstance().showExtraDescriptionInfo())
            {
                //text = text + "Pending Visitors: " + mPendingVisitors;
                text = text + "\n" + "Stay Time: " + TimesSettings.newStayTime.ToString("F0") + " seconds";
            }
            return text;
        }
    }
    internal class ShowRemainingTime
    {
        
        /*public static string GetRemainingTime(MerchantShip ship)
        {
            if (ship != null)
            {
                TimeSpan timeLeft = ship.getTimeLeft();
                return string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
            }
            return "00:00:00";
        }*/
    }
}
