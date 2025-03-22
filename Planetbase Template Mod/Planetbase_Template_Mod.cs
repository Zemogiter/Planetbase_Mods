using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Planetbase_Template_Mod
{
    public class Planetbase_Template_Mod : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new Planetbase_Template_Mod(), modEntry, "" + typeof(Planetbase_Template_Mod).Name);

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
}
