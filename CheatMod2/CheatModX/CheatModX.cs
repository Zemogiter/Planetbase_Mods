using Planetbase;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using Module = Planetbase.Module;
using System;

namespace CheatModX;

public class CheatModX : ModBase
{
    public static new void Init(ModEntry modEntry) => InitializeMod(new CheatModX(), modEntry, "CheatModX");

    public override void OnInitialized(ModEntry modEntry)
    {
        Resources.UnloadUnusedAssets();
        new GameObject().AddComponent<ModControlador>();
        Debug.Log("[MOD] CheatModX activated");
    }

    public override void OnUpdate(ModEntry modEntry, float timeStep)
    {
        //nothing needed here for now
    }
}
