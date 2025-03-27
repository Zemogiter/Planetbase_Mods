using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using Module = Planetbase.Module;

namespace FreeFurnishing
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Enable Unsafe Mode?")] public bool unsafeMode = false;
        [Draw("Rotate by increments?")] public bool rotateByIncrements = true;
        [Draw("Rotation angle increment (degrees)")] public float rotationAngle = 15f;
        [Draw("Rotate Clockwise Keybind")] public KeyCode rotateUpKeybind = KeyCode.X;
        [Draw("Rotate Counter-Clockwise Keybind")] public KeyCode rotateDownKeybind = KeyCode.Z;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class FreeFurnishing : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry) 
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new FreeFurnishing(), modEntry, "FreeFurnishing"); 
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
    [HarmonyPatch(typeof(Module), nameof(Module.canPlaceComponent))]
    public class ModulePatch
    { 
        public static bool Prefix(Module __instance, ref bool __result, ConstructionComponent component)
        {
            __result = ReplacementMethod(__instance, component);
            return false;
        }
        public static bool ReplacementMethod(Module __instance, ConstructionComponent component)
        {
            List<ComponentType> forbiddenComponents =
            [
                new VideoScreen(),
                //new SickBayBed(),
                //new MedicalCabinet(),
                //new SecurityConsole(),
                //new Armory(),
                //new TelescopeConsole(),
                //new RadioConsole(),
                //new Bed(),
                //new Bunk(),
                //new Workbench(),
                //new TissueSynthesizer()
            ];
            //checking if selected component is on the excluded list
            if (FreeFurnishing.settings.unsafeMode == true && !forbiddenComponents.Contains(component.getComponentType()))
            {
                if (FreeFurnishing.settings.rotateByIncrements == true)
                {
                    //rotation by increments
                    if (InputAction.isValidKey(FreeFurnishing.settings.rotateUpKeybind))
                    {
                        component.getTransform().Rotate(Vector3.up * FreeFurnishing.settings.rotationAngle);
                    }
                    if (InputAction.isValidKey(FreeFurnishing.settings.rotateDownKeybind))
                    {
                        component.getTransform().Rotate(Vector3.down * FreeFurnishing.settings.rotationAngle);
                    }
                }
                else
                {
                    //continious rotation while key is pressed
                    if (InputAction.isValidKey(FreeFurnishing.settings.rotateUpKeybind))
                    {
                        component.getTransform().Rotate(Vector3.up * 5f);
                    }
                    if (InputAction.isValidKey(FreeFurnishing.settings.rotateDownKeybind))
                    {
                        component.getTransform().Rotate(Vector3.down * 5f);
                    }
                }

                Vector3 fromCenter = component.getPosition() - __instance.getPosition();
                fromCenter.x = Mathf.Round(fromCenter.x);
                fromCenter.z = Mathf.Round(fromCenter.z);
                component.setPosition(component.getPosition() + fromCenter);
            }
            else
            {
                bool flag = true;
                var componentLocations = CoreUtils.GetMember<Module, SimpleTransform[]>("mComponentLocations", __instance);
                if (componentLocations != null)
                {
                    CoreUtils.InvokeMethod<Module>("clampComponentPosition", __instance, component);
                }
                else
                {
                    CoreUtils.InvokeMethod<Module>("clampComponentPosition", __instance, component);
                    flag = __instance.isValidLayoutPosition(component);
                }
                return !CoreUtils.InvokeMethod<Module, bool>("intersectsAnyComponents", __instance, component);
            }
            
            CoreUtils.InvokeMethod<Module>("clampComponentPosition", __instance, component);

            return !CoreUtils.InvokeMethod<Module, bool>("intersectsAnyComponents", __instance, component);
        }
    }
}
