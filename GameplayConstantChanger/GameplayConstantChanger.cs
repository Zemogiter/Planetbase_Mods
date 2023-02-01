using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameplayConstantChanger
{
    public class GameplayConstantChanger : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new GameplayConstantChanger(), modEntry, "GameplayConstantChanger");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
}
