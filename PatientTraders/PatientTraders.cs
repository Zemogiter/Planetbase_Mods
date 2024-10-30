using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace PatientTraders
{
    [DrawFields(DrawFieldMask.Public)]
    public class TimesSettings
    {
        public static float newStayTime = 560f;
        public static float newTradeTime = 6800f;
        public static float newStateTime = 0f;
    }    
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Settings", Collapsible = true)] public TimesSettings TimesSettings = new();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }
    public class PatientTraders : ModBase
    {

        public static new void Init(ModEntry modEntry) => InitializeMod(new PatientTraders(), modEntry, "PatientTraders");

        public override void OnInitialized(ModEntry modEntry)
        {
            
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
        
    }
}
