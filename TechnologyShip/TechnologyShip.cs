using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TechnologyShip
{
    public class TechnologyShip : ModBase
    {
        public enum MerchantCategory
        {
            RawMaterial,
            Medical,
            Food,
            Industrial,
            Electronics,
            Count
        }
        MerchantCategory Technology = (MerchantCategory)6;

        public static new void Init(ModEntry modEntry) => InitializeMod(new TechnologyShip(), modEntry, "TechnologyShip");

        public override void OnInitialized(ModEntry modEntry)
        {
            if (SomeValue == (MerchantCategory)6)
            {

            }
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }

    }
    
    public class NewMerchant : MerchantShip
    {
        public enum MerchantCategory
        {

        }
    }
}
