using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace RemoveTutorialFromMenu
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Debug mode")] public bool debugMode = false;
        [Draw("Spread menu buttons fuirther apart")] public bool spreadMenuButtons = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class RemoveTutorialFromMenu : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            if (settings.debugMode) Harmony.DEBUG = true;
            InitializeMod(new RemoveTutorialFromMenu(), modEntry, "RemoveTutorialFromMenu");
            Harmony harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
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
            Profile.getInstance().setTutorialPlayed();
            Profile.getInstance().setTutorialCompleted();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing to do here
        }
    }
    [HarmonyPatch(typeof(GameStateTitle), "onGui")]
    [HarmonyDebug]
    public class MainMenuPatch
    {
        //main code that removes the tutorial button from the main menu
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (RemoveTutorialFromMenu.settings.debugMode) Console.WriteLine("MainMenuPatch Transpiler is being executed.");

            var codes = new List<CodeInstruction>(instructions);
            MethodBase targetMethod = typeof(GameManager).GetMethod("setGameStateGameNew", BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < codes.Count; i++)
            {
                // Identify the start of the if block
                if (i + 10 < codes.Count && codes[i + 10].opcode == OpCodes.Ldstr && codes[i + 10].operand.ToString() == "tutorial")
                {
                    // Remove the if block instructions
                    int endIndex = i + 3;
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                    while (endIndex < codes.Count && !(codes[endIndex].opcode == OpCodes.Callvirt && codes[endIndex].operand == targetMethod))
                    {
                        endIndex++;
                    }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                    if (endIndex < codes.Count)
                    {
                        var rangeToRemove = codes.GetRange(i, endIndex - i + 1);
                        codes.RemoveRange(i, endIndex - i + 1);

                        // Adjust branch targets
                        for (int j = 0; j < codes.Count; j++)
                        {
                            if (codes[j].operand is Label label)
                            {
                                int targetIndex = codes.FindIndex(ci => ci.labels.Contains(label));
                                if (targetIndex >= i && targetIndex <= endIndex)
                                {
                                    if (codes[endIndex].labels.Any())
                                    {
                                        codes[j].operand = codes[endIndex].labels.First();
                                    }
                                    else
                                    {
                                        // Handle case where there are no labels
                                        Label newLabel = new Label();
                                        codes[endIndex].labels.Add(newLabel);
                                        codes[j].operand = newLabel;
                                    }
                                }
                            }
                        }
                        if (RemoveTutorialFromMenu.settings.debugMode)
                        {
                            Console.WriteLine(endIndex - i + 1 + " instructions removed.");
                            foreach (var instruction in rangeToRemove)
                            {
                                Console.WriteLine("Removed instruction: " + instruction.ToString());
                            }
                        }
                    }
                    break;
                }
            }
            return codes.AsEnumerable();
        }
        //secondary code that spreads the menu buttons further apart
        static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            if (RemoveTutorialFromMenu.settings.debugMode) Console.WriteLine("MainMenuPatch Transpiler2 is being executed.");

            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 1.3f && RemoveTutorialFromMenu.settings.spreadMenuButtons)
                {
                    codes[i].operand = 4.0f;
                    if (RemoveTutorialFromMenu.settings.debugMode)
                    {
                        Console.WriteLine("Changed instruction: " + codes[i].ToString());
                    }
                    break;
                }
            }
            return codes.AsEnumerable();
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
            MethodInfo methodInfo = GetPrivateMethod(obj, methodName, instance) ?? throw new MissingMethodException($"Could not find \"{methodName}\"");
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
            FieldInfo fieldInfo = GetPrivateField(obj, fieldName, instance) ?? throw new MissingMethodException($"Could not find \"{fieldName}\"");
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
}
