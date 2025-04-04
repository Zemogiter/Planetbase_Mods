using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Planetbase;

namespace PatientTraders
{
    
    [HarmonyPatch(typeof(MerchantShip))]
    internal class StayTimePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (PatientTraders.settings.changeStayTime == true)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 180f)
                    {
                        codes[i].operand = TimesSettings.newStayTime;
                        break;
                    }
                }
                return codes.AsEnumerable();
            }
            return instructions;
        }
        static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            if (PatientTraders.settings.changeTradeTime == true)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 1200f)
                    {
                        codes[i].operand = TimesSettings.newTradeTime;
                        break;
                    }
                }
                return codes.AsEnumerable();
            }
            return instructions;
        }
        /*
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
        */
    }

}
