using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;

namespace PatientTraders
{
    //main code, changing stay time and trade time
    internal class StayTimePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (PatientTraders.settings.changeStayTime == true)
            {
                var codes = new List<CodeInstruction>(instructions);
                int i;
                for (i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 180f)
                    {
                        codes[i].operand = TimesSettings.newStayTime;
                        if (PatientTraders.settings.debugMode) Console.WriteLine("PatientTraders - changing stay time from 180 to " + codes[i].operand.ToString());
                        break;
                    }
                }
                if (i == codes.Count)
                { Console.WriteLine("PatientTraders - couldn't find stay time code to change");
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
                int i;
                for (i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 1200f)
                    {
                        codes[i].operand = TimesSettings.newTradeTime;
                        if (PatientTraders.settings.debugMode) Console.WriteLine("PatientTraders - changing trade time from 1200 to " + codes[i].operand.ToString());
                        break;
                    }
                }
                if (i == codes.Count)
                {
                    Console.WriteLine("PatientTraders - couldn't find trade time code to change");
                }
                return codes.AsEnumerable();
            }
            return instructions;
        }
    }
    /*
    [HarmonyPatch(typeof(MerchantShip), nameof(MerchantShip.update))]
    internal class DebugPatch
    {
        public static void Postfix(MerchantShip __instance)
        {
            if (PatientTraders.settings.debugMode == true)
            {
                var stayTime = CoreUtils.GetMember<MerchantShip, float>("StayTime", __instance);
                Console.WriteLine("PatientTraders - vanilla stay time is 180, current one is: " + stayTime.ToString());
                var tradeTime = CoreUtils.GetMember<MerchantShip, float>("TradeTime", __instance);
                Console.WriteLine("PatientTraders - vanilla trade time is 1200, current one is: " + tradeTime.ToString());
            }
        }
    }
    */
}
