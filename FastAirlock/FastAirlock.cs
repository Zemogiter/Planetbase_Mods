using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace FastAirlock
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Speed multiplier for airlock animations")] public float speedmult = 2;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class FastAirlock : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new FastAirlock(), modEntry, "FastAirlock");
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
            //nothing required here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //nothing required here
        }
    }
    //this patch is needed so players wont be able to decostruct airlocks when used, causing the game to crash with an NRE in Selectable class
    [HarmonyPatch(typeof(Construction), nameof(Construction.isDeleteable))]
    public class ModuleAirlockDeconstructionPatch
    {
        public static bool Prefix(Module __instance, out bool buttonEnabled)
        {
            if (__instance.getModuleType() == TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeAirlock>() && __instance.getInteractionCount() > 0)
            {
                buttonEnabled = Grid.isSplitterConstruction(__instance);
                return false;
            }
            buttonEnabled = !Grid.isSplitterConstruction(__instance);
            return true;
        }
    }
    //main patch
    [HarmonyPatch(typeof(InteractionAirlock), nameof(InteractionAirlock.update))]
    public class InteractionAirlockPatch
    {
        public static void Prefix(ref float timeStep)
        {
            timeStep = FastAirlock.settings.speedmult;
        }
    }
}