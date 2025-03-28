using System.Reflection;
using HarmonyLib;
using Planetbase;

namespace PatientTraders
{
    [HarmonyPatch(typeof(MerchantShip))]
    internal class StayTimePatch
    {
        public readonly FieldInfo _StayTimeField = typeof(MerchantShip).GetField("StayTime", BindingFlags.NonPublic | BindingFlags.Instance);
        public int newStayTime
        {
            get
            {
                return (int)_StayTimeField.GetValue(this);
            }
            set
            {
                _StayTimeField.SetValue(this, TimesSettings.newStayTime);
            }
        }
       
    }
    [HarmonyPatch(typeof(MerchantShip))]
    internal class TradeTimePatch
    {
        public readonly FieldInfo _TradeTimeField = typeof(MerchantShip).GetField("TradeTime", BindingFlags.NonPublic | BindingFlags.Instance);
        public int newTradeTime
        {
            get
            {
                return (int)_TradeTimeField.GetValue(this);
            }
            set
            {
                _TradeTimeField.SetValue(this, TimesSettings.newTradeTime);
            }
        }
    }
    [HarmonyPatch(typeof(LandingShip))]
    internal class LandingShipPatch
    {
        public readonly FieldInfo _StateTimeField = typeof(LandingShip).GetField("StateTime", BindingFlags.NonPublic | BindingFlags.Instance);
        public int newStateTime
        {
            get
            {
                return (int)_StateTimeField.GetValue(this);
            }
            set
            {
                _StateTimeField.SetValue(this, TimesSettings.newStateTime);
            }
        }
    }
}
