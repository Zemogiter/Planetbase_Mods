using HarmonyLib;
using PauseMod.Patches.GuiMenuSystem;

namespace PauseMod.Patches
{
    [HarmonyPatch(typeof(global::Planetbase.TimeManager))]
    [HarmonyPatch("isPaused")]
    public class IsPausedPatch
    {
        private static readonly bool ShouldPatch = PauseMod.ShouldOverrideTimeManagerIsPaused;

        public static bool Postfix(bool __result)
        {
            if (ShouldPatch)
                return __result && !InitPatch.WasManuallyPaused;

            return __result;
        }
    }
}
