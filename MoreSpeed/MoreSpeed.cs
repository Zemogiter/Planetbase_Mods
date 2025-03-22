using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace MoreSpeed
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Enable Unsafe Mode?")] public bool UnsafeMode = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class MoreSpeed : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new MoreSpeed(), modEntry, "MoreSpeed");
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;

            return true;
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            //not used
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //not used
        }
        public override void OnGameStart(GameStateGame gameStateGame)
        {
            UpdateTimeScale();
        }
        private void UpdateTimeScale()
        {
            TimeManager instance = Singleton<TimeManager>.getInstance();
            List<float> list = (instance.GetPrivateFieldValue<object>("TimeScales") as IEnumerable<float>).ToList();
            list.Add(6f);
            list.Add(8f);
            //10 speed glitches out ship landing, making them hover over landing pads/starports indefinitly, therefore it's not avaialble by default
            if (MoreSpeed.settings.UnsafeMode)
            {
                list.Add(10f);
            }
            if(!MoreSpeed.settings.UnsafeMode && list.Contains(12f))
            {
                list.Remove(12f);
            }
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
