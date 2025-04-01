using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Text;
using System.Windows.Input;
using UnityEngine;
using UnityModManagerNet;

namespace BetterMilestones
{
    public class Settings : ModSettings, IDrawable
    {
        [Draw("Debug mode")] public bool debugMode = true;
        [Draw("Smooth exit from milestone cinematic?(currently nonfunctional)")] public bool smoothExit = true;
        [Draw("Change the Self Sufficency milestone to include Factory?")] public bool changeSS = true;
        [Draw("Change the Robotization milestone to have higher bot count requirement?")] public bool changeR = true;
        [Draw("New bot count requirement.")] public int botCount = 30;
        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class BetterMilestones : ModBase
    {
        public static bool enabled;
        public static Settings settings;
        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            InitializeMod(new BetterMilestones(), modEntry, "BetterMilestones");
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
    //HarmonyPatch for quiting the Milestone cinematic on button input
    [HarmonyPatch(typeof(MilestoneCinemetic), nameof(MilestoneCinemetic.update))]
    public class MilestoneCameraPatch
    {
        public static void Prefix(MilestoneCinemetic __instance)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //for future reference, use UnityEngine.InputLegacyModule for checking if a keybind was activated, more likey to work than InputAction
            {
                if (BetterMilestones.settings.smoothExit == true) //to-do: figure out how to get this to work
                {
                    if (BetterMilestones.settings.debugMode) Console.WriteLine("BetterMilestones - smooth transition activated");
                    CoreUtils.SetMember<MilestoneCinemetic, float>("mTime", __instance, 14f);
                    Singleton<TimeManager>.getInstance().unpause();
                    bool tops = CoreUtils.GetMember<MilestoneCinemetic, bool>("mRenderTopsState", __instance);
                    if (BetterMilestones.settings.debugMode) Console.WriteLine("The value of tops is: " + tops);
                    Construction.setRenderTops(false, force: true);
                    CoreUtils.SetMember<MilestoneCinemetic,bool>("mRestoredTops", __instance, true);
                }
                CameraManager.getInstance().setCinematic(null);
            }
        }
    }
    //HarmonyPatch that moddifies requirements for Self Sufficency milestone (Factory is pretty important)
    [HarmonyPatch(typeof(MilestoneSelfSufficiency), methodType: MethodType.Constructor)]
    public class RequirementPatch
    {
        public static void Postfix(MilestoneSelfSufficiency __instance)
        {
            if (BetterMilestones.settings.changeSS == true)
            {
                // Handle mRequiredComponents
                var componentList = CoreUtils.GetMember<Milestone, ComponentType[]>("mRequiredComponents", __instance);
                var newComponentList = new ComponentType[componentList.Length + 2];
                Array.Copy(componentList, newComponentList, componentList.Length);
                newComponentList[componentList.Length] = TypeList<ComponentType, ComponentTypeList>.find<SemiconductorFoundry>() as ComponentType;
                newComponentList[componentList.Length + 1] = TypeList<ComponentType, ComponentTypeList>.find<SparesWorkshop>() as ComponentType;
                CoreUtils.SetMember<Milestone, ComponentType[]>("mRequiredComponents", __instance, newComponentList);

                if (BetterMilestones.settings.debugMode)
                {
                    foreach (var component in newComponentList)
                    {
                        Console.WriteLine("BetterMilestones - outputting componentList: " + component.getName());
                    }
                }

                // Handle mRequiredModules
                var moduleList = CoreUtils.GetMember<Milestone, ModuleType[]>("mRequiredModules", __instance);
                var newModuleList = new ModuleType[moduleList.Length + 1];
                Array.Copy(moduleList, newModuleList, moduleList.Length);
                newModuleList[moduleList.Length] = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeFactory>() as ModuleType;
                CoreUtils.SetMember<Milestone, ModuleType[]>("mRequiredModules", __instance, newModuleList);

                if (BetterMilestones.settings.debugMode)
                {
                    foreach (var module in newModuleList)
                    {
                        Console.WriteLine("BetterMilestones - outputting moduleList: " + module.getName());
                    }
                }
            }
        }
    }
    //HarmonyPatch that moddifies requirements for Robotization milestone (10 bots is nothing)
    [HarmonyPatch(typeof(MilestoneRobotization), methodType: MethodType.Constructor)]
    public class BotCountPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            if (BetterMilestones.settings.changeR == true)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 10)
                    {
                        codes[i].operand = BetterMilestones.settings.botCount;
                        break;
                    }
                }
                return codes.AsEnumerable();
            }
            return instructions;
        }
    }
}
