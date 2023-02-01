using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;

namespace ExteriorO2Genereator
{
    [HarmonyPatch(typeof(Connection), nameof(Connection))]
    internal class OxygenCables
    {

    }
}
