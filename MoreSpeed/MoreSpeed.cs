using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

namespace MoreSpeed
{
    public class MoreSpeed : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new MoreSpeed(), modEntry, "MoreSpeed");

        public override void OnInitialized(ModEntry modEntry)
        {
            UpdateTimeScale();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
        private void UpdateTimeScale()
        {
            TimeManager instance = Singleton<TimeManager>.getInstance();
            List<float> list = (instance.GetPrivateFieldValue<object>("TimeScales") as IEnumerable<float>).ToList();
            list.Add(6f);
            list.Add(8f);
            //10 speed glitches out ship landing, making them hover over landing pads/starports indefinitly
            //list.Add(10f);
            instance.SetPrivateFieldValue("TimeScales", list.ToArray());
            instance.GetPrivateFieldValue<object>("TimeScales");
        }
    }
    public static class ModExtensions
    {
        public static readonly BindingFlags BindingFlagsEverything = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static T GetPrivateFieldValue<T>(this object obj, string fieldName) where T : class
        {
            return obj.GetType().GetField(fieldName, BindingFlagsEverything).GetValue(obj) as T;
        }

        public static object GetPrivateFieldValue(this object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName, BindingFlagsEverything).GetValue(obj);
        }

        public static void SetPrivateFieldValue<T>(this object obj, string fieldName, T newValue)
        {
            obj.GetType().GetField(fieldName, BindingFlagsEverything).SetValue(obj, newValue);
        }
    }
}
