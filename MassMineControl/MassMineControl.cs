using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace MassMineControl
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Disable all mines")] public KeyCode disableMines = KeyCode.Alpha9;
        [Draw("Enable all mines")] public KeyCode enableMines = KeyCode.Alpha0;
        [Draw("Make all mines high-priority")] public KeyCode highPriorityMines = KeyCode.LeftBracket;
        [Draw("Make all mines normal-priority")] public KeyCode normalPriorityMines = KeyCode.RightBracket;
        [Draw("Display confirmation messages?")] public bool displayMessages = true;
        [Draw("Ignore currently selected module?")] public bool ignoreSelected = false;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class MassMineControl : ModBase
    {
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new MassMineControl(), modEntry, "MassMineControl");
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

        public const string MESSAGE = "All mines disabled.";
        public const string MESSAGE2 = "All mines enabled.";
        public const string MESSAGE3 = "Setting all mines to high-priority.";
        public const string MESSAGE4 = "Setting all mines to normal-priority.";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
        public void RegisterStrings()
        {
            StringUtils.RegisterString("message_mine_disable", MESSAGE);
            StringUtils.RegisterString("message_mine_enable", MESSAGE2);
            StringUtils.RegisterString("message_mine_high_priority", MESSAGE3);
            StringUtils.RegisterString("message_mine_normal_priority", MESSAGE4);
        }
    }
    [HarmonyPatch(typeof(Module), nameof(Module.update))]
    public class ModulePatch
    {
        static void Postfix()
        {
            var originalList = BuildableUtils.GetAllModules();
            var mineType = BuildableUtils.FindModuleType<ModuleTypeMine>();
            var mineList = originalList.Where(a => a.getModuleType() == mineType).ToList();

            if (MassMineControl.settings.ignoreSelected == true)
            {
                //To-do: find a way to access currently selected module
                var currentlySelected = 
                mineList.Remove();
            }

            foreach (Module mine in mineList)
            {
                if (Input.GetKeyUp(MassMineControl.settings.disableMines) && mine.isEnabled() == true)
                {
                    mine.setEnabled(false);
                    if (MassMineControl.settings.displayMessages == true)
                    {
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_mine_disable", MassMineControl.MESSAGE), ResourceList.StaticIcons.Disable, 1));
                    }
                }
                if (Input.GetKeyUp(MassMineControl.settings.enableMines) && mine.isEnabled() == false)
                {
                    mine.setEnabled(true);
                    if (MassMineControl.settings.displayMessages == true)
                    {
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_mine_enable", MassMineControl.MESSAGE2), ResourceList.StaticIcons.Enable, 1));
                    }
                }
                if (Input.GetKeyUp(MassMineControl.settings.highPriorityMines) && !mine.isHighPriority())
                {
                    mine.setHighPriority(true);
                    if (MassMineControl.settings.displayMessages == true)
                    {
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_mine_high_priority", MassMineControl.MESSAGE3), ResourceList.StaticIcons.PriorityUp, 1));
                    }
                }
                if (Input.GetKeyUp(MassMineControl.settings.normalPriorityMines) && mine.isHighPriority())
                {
                    mine.setHighPriority(false);
                    if (MassMineControl.settings.displayMessages == true)
                    {
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_mine_normal_priority", MassMineControl.MESSAGE4), ResourceList.StaticIcons.PriorityDown, 1));
                    }
                }
            }
        }
    }
}
