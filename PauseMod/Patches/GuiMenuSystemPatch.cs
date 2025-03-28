using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityModManagerNet;

namespace PauseMod.Patches.GuiMenuSystem
{
    [HarmonyPatch(typeof(global::Planetbase.GuiMenuSystem))]
    [HarmonyPatch("init")]
    public class InitPatch
    {
        public static bool WasManuallyPaused { get; private set; }

        public static void Postfix(global::Planetbase.GuiMenuSystem __instance)
        {
            var timeManager = Singleton<TimeManager>.getInstance();

            string assembly = Assembly.GetExecutingAssembly().Location;
            string filename = Path.GetDirectoryName(assembly);

            string path = Path.Combine(filename + "\\assets\\pause button.png");
            //Console.WriteLine("Icon path: " + path);
            Texture2D icon;
            if (File.Exists(path))
            {
                icon = UnityModManagerNet.Utils.LoadTexture(path);
                Util.applyColor(icon);
            }
            else
            {
                icon = ResourceList.StaticIcons.Achievements;
            }
            var pauseMenuItem = new GuiMenuItem(
                icon,
                StringList.get("menu_pause"),
                parameter =>
                {
                    var tmInstance = TimeManager.getInstance();
                    if (tmInstance.isPaused() || InputAction.isValidKey(PauseMod.settings.pauseKeybind)) //generic unpause
                    {
                        Console.WriteLine(Assembly.GetExecutingAssembly().FullName + " Unpausing");
                        tmInstance.unpause();
                    }
                    if (InputAction.isValidKey(PauseMod.settings.pauseKeybind) || !tmInstance.isPaused()) //generic pause
                    {
                        Console.WriteLine(Assembly.GetExecutingAssembly().FullName + " Pausing");
                        tmInstance.pause();
                    }
                }
            );
            //Add the new button to the main GUI menu and reorder it
            GuiMenu member = CoreUtils.GetMember<global::Planetbase.GuiMenuSystem, GuiMenu>("mMenuSpeed", __instance);
            member.AddItemBeforeBackItem(pauseMenuItem);
        }
    }
}