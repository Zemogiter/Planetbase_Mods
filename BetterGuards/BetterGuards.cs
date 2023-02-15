using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Data;
using UnityModManagerNet;
using System.Reflection;

namespace BetterGuards
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Enable gun drops?")] public bool enableGunDrops = true;
        [Draw("Health multipler for guards")] public float healthmult = 4.2f;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class BetterGuards : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new BetterGuards(), modEntry, "BetterGuards");
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
            //nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing needed here
        }
    }
    public static class Reflection
    {
        public static MethodInfo GetPrivateMethod(Type obj, string methodName, bool instance)
        {
            try
            {
                BindingFlags flags = BindingFlags.NonPublic | ((instance) ? BindingFlags.Instance : BindingFlags.Static);
                return obj.GetMethod(methodName, flags);
            }
            catch
            {
                return null;
            }
        }

        public static bool TryGetPrivateMethod(Type obj, string methodName, bool instance, out MethodInfo methodInfo)
        {
            methodInfo = GetPrivateMethod(obj, methodName, instance);
            return (methodInfo != null);
        }

        public static MethodInfo GetPrivateMethodOrThrow(Type obj, string methodName, bool instance)
        {
            MethodInfo methodInfo = GetPrivateMethod(obj, methodName, instance);
            if (methodInfo == null)
                throw new MissingMethodException($"Could not find \"{methodName}\"");

            return methodInfo;
        }

        public static FieldInfo GetPrivateField(Type obj, string fieldName, bool instance)
        {
            try
            {
                BindingFlags flags = BindingFlags.NonPublic | ((instance) ? BindingFlags.Instance : BindingFlags.Static);
                return obj.GetField(fieldName, flags);
            }
            catch
            {
                return null;
            }
        }

        public static FieldInfo GetPrivateFieldOrThrow(Type obj, string fieldName, bool instance)
        {
            FieldInfo fieldInfo = GetPrivateField(obj, fieldName, instance);
            if (fieldInfo == null)
                throw new MissingMethodException($"Could not find \"{fieldName}\"");

            return fieldInfo;
        }

        public static bool TryGetPrivateField(Type obj, string fieldName, bool instance, out FieldInfo fieldInfo)
        {
            fieldInfo = GetPrivateField(obj, fieldName, instance);
            return (fieldInfo != null);
        }

        public static object InvokeStaticMethod(MethodInfo method, params object[] args)
        {
            return method.Invoke(null, args);
        }

        public static object InvokeInstanceMethod(object instance, MethodInfo method, params object[] args)
        {
            return method.Invoke(instance, args);
        }

        public static object GetStaticFieldValue(FieldInfo field)
        {
            return field.GetValue(null);
        }

        public static object GetInstanceFieldValue(object instance, FieldInfo field)
        {
            return field.GetValue(instance);
        }

        public static void SetStaticFieldValue(FieldInfo field, object value)
        {
            field.SetValue(null, value);
        }

        public static void SetInstanceFieldValue(object instance, FieldInfo field, object value)
        {
            field.SetValue(instance, value);
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.init))]
    public class GuardPatch
    {
        //replacing the default health bar with a larger one for guards
        public static void Postfix(Character __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>())
            {
                Indicator indicator = new(StringList.get("health"), ResourceList.StaticIcons.Health, IndicatorType.Normal, 1f, BetterGuards.settings.healthmult, SignType.Health);
                Console.WriteLine("The new indicator is " + indicator);
                indicator.setLevels(0.1f, 0.5f, 0.7f, 0.8f);
                Console.WriteLine("New indicator's levels " + indicator.getLevels());
                indicator.setOrientation(IndicatorOrientation.Vertical);
                CharacterIndicator healthIndicator = CharacterIndicator.Health;
                __instance.getIndicator(healthIndicator).setValue(indicator.getMax());
                Console.WriteLine("This should be the same as the new indicator " + __instance.mIndicators[0]);
            }
        }
    }
    [HarmonyPatch(typeof(Character), nameof(Character.onKo))]
    public class CharacterPatch
    {
        //make guards and intruders drop guns on death (vanilla = guns disapear with them)
        public static void Postfix(Character __instance)
        {
            if(BetterGuards.settings.enableGunDrops == true && __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Guard>() || __instance.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Intruder>())
            {
                Vector3 position = __instance.getPosition();
                Location location = __instance.getLocation();
                ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();
                Resource droppedGun = Resource.create(gunType, position, location);
            }
        }
    }
}
