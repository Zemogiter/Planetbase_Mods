using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Planetbase;
using static UnityModManagerNet.UnityModManager;

namespace PatientTraders
{
    [HarmonyPatch(typeof(MerchantShip))]
    internal class StayTimePatch
    {
        public readonly FieldInfo _StayTimeField = typeof(MerchantShip).GetField("StayTime", BindingFlags.NonPublic | BindingFlags.Instance);
        public int newStay
        {
            get
            {
                return (int)_StayTimeField.GetValue(this);
            }
            set
            {
                //_StayTimeField.SetValue(this, PatientTraders.newStayTime);
                _StayTimeField.SetValue(this, TimesSettings.newStayTime);
                //_StayTimeField.SetValue(this, Settings.newStayTime);
            }
        }
       
    }
    [HarmonyPatch(typeof(MerchantShip))]
    internal class TradeTimePatch
    {
        public readonly FieldInfo _TradeTimeField = typeof(MerchantShip).GetField("TradeTime", BindingFlags.NonPublic | BindingFlags.Instance);
        public int newTrade
        {
            get
            {
                return (int)_TradeTimeField.GetValue(this);
            }
            set
            {
                //_TradeTimeField.SetValue(this, PatientTraders.newTradeTime);
                _TradeTimeField.SetValue(this, TimesSettings.newTradeTime);
                //_TradeTimeField.SetValue(this, Settings.newTradeTime);
            }
        }
    }
    [HarmonyPatch(typeof(LandingShip))]
    internal class LandingShipPatch
    {
        public readonly FieldInfo _StateTimeField = typeof(LandingShip).GetField("StateTime", BindingFlags.NonPublic | BindingFlags.Instance);
        public int newState
        {
            get
            {
                return (int)_StateTimeField.GetValue(this);
            }
            set
            {
                //_StateTimeField.SetValue(this, PatientTraders.newStateTime);
                _StateTimeField.SetValue(this, TimesSettings.newStateTime);
                //_StateTimeField.SetValue(this, Settings.newStateTime);
            }
        }
    }
}
