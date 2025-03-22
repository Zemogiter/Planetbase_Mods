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
    [HarmonyPatch(typeof(AudioPlayer), nameof(AudioPlayer))]
    public class AddMusic
    {
        public static Prefix()
        {
            mParentSourceObject = new GameObject();
            mParentSourceObject.name = "Audio Sources";
            mMusicSlot = new AudioSlot(mParentSourceObject, "Music Source");
            mAudioSlots = new AudioSlot[32];
            for (int i = 0; i < 32; i++)
            {
                mAudioSlots[i] = new AudioSlot(mParentSourceObject, "Source " + i);
            }
            mMusicVolume = Singleton<Profile>.getInstance().getMusicVolumeNormalized();
            mSfxVolume = Singleton<Profile>.getInstance().getSfxVolumeNormalized();
        }
    }
}
