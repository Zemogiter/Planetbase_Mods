using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using System.Reflection;

namespace AutoDisableColonistShips
{
    public class AutoDisableColonistShips : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new AutoDisableColonistShips(), modEntry, "AutoDisableColonistShips");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
        }
    }
    [HarmonyPatch(typeof(LandingPermissions), nameof(LandingPermissions.areColonistsAllowed))]
    public class LandingPermissionsPatch : LandingPermissions
    {
        public static bool Prefix()
        {
            RefBool refbool = ModExtensions.GetPrivateFieldValue<LandingPermissions>(bool, "mVisitorsAllowed");
            if (GameManager.getInstance().getGameState() is GameStateGame game)
            {
                int triggerValue = 4;
                int numberofColonists = Character.getHumanCount();
                int maxNumber = Planetbase.Module.getOverallOxygenGeneration();

                if (maxNumber - numberofColonists < triggerValue || maxNumber == numberofColonists)
                {
                    //code to disable arrival of colonist ships if we have less than 4 left in oxygen production
                    //LandingPermissions landing = new LandingPermissions();
                    //landing.getColonistRefBool().set(false);
                    return refbool.set(false);
                }
            }
            return true;
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
