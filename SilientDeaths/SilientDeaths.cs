using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace SilientDeaths
{
    public class SilientDeaths : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new SilientDeaths(), modEntry, "SilientDeaths");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(AudioPlayer), nameof(AudioPlayer.play))]
    public class SoundPatch
    {
        static bool Prefix(AudioPlayer __instance, SoundDefinition ___definition, Selectable ___selectable, bool ___loop)
        {
            //To-do: figure out how to get a Character source of the sound and mute it
            if (___selectable.getGameObject() is not)
            {

            }
            else
            {

            }
            return false;    
        }
    }
}
