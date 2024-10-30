using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace CustomMusic
{
    public class CustomMusic : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new CustomMusic(), modEntry, "CustomMusic");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(AudioPlayer), nameof(AudioPlayer.update))]
    public class AddMusic
    {
        static void Postfix()
        {

        }
    }
}
