using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace TechnologyShip
{
    public class TechnologyShip : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new TechnologyShip(), modEntry, "TechnologyShip");

        public override void OnInitialized(ModEntry modEntry)
        {
            
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }

    }
    
    public class NewMerchant : MerchantShip
    {
        
    }
}
