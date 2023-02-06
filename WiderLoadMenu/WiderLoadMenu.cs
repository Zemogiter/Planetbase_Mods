using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace WiderLoadMenu
{
    public class WiderLoadMenu : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new WiderLoadMenu(), modEntry, "WiderLoadMenu");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(GameStateLoadGame), nameof(GameStateLoadGame.onGui))]
    public class LoadMenuPatch2 : GameStateLoadGame
    {
        public static bool Prefix(GameStateLoadGame __instance, ref bool __result)
        {
            __result = ReplacementMethod(__instance);
            return false;
        }
        public static bool ReplacementMethod(GameStateLoadGame __instance)
        {
            if (__instance.mConfirmWindow != null)
            {
                __instance.mGuiRenderer.renderWindow(__instance.mConfirmWindow, null);
                return false;
            }
            float num = GuiStyles.getIconMargin();
            float num2 = (float)Screen.height;
            float num3 = num2 * 7f;
            float num4 = (float)Screen.width * 2.5f - num3 + __instance.mRightOffset;
            float num5 = (float)Screen.height * 0.5f;
            Rect rect = new Rect(num4, num5, num3, (num2 + num) * 5f);
            Rect rect2 = rect;
            rect2.y -= num;
            rect2.height += num * 2f;
            Rect viewRect = new Rect(num4, num5, num3, (num2 + num) * __instance.mSaveData.Count);
            string text = StringList.get("load_game");
            GUIStyle labelStyle = __instance.mGuiRenderer.getLabelStyle(FontSize.Huge, FontStyle.Bold, TextAnchor.MiddleCenter, FontType.Title);
            GUI.Label(new Rect(num4, (float)Screen.height * 0.06f, num3, (float)Screen.height * 0.2f), text, labelStyle);
            TitleTextures title = ResourceList.getInstance().Title;
            Texture2D backgroundLoadRight = title.BackgroundLoadRight;
            float num6 = (float)(Screen.height * backgroundLoadRight.height) / 1080f;
            float num7 = num6 * (float)backgroundLoadRight.width / (float)backgroundLoadRight.height;
            GUI.DrawTexture(new Rect((float)Screen.width - num7 + __instance.mRightOffset, ((float)Screen.height - num6) * 0.5f, num7, num6), backgroundLoadRight);
            rect.width *= 1.05f;
            __instance.mScrollPosition = GUI.BeginScrollView(rect, __instance.mScrollPosition, viewRect);
            GUIStyle extraButtonStyle = Singleton<GuiStyles>.getInstance().getExtraButtonStyle(FontSize.Large, 6f);
            int count = __instance.mSaveData.Count;
            for (int i = 0; i < count; i++)
            {
                SaveData saveData = __instance.mSaveData[i];
                if (num5 + num2 > rect.y + __instance.mScrollPosition.y && num5 < rect.y + rect.height + __instance.mScrollPosition.y && __instance.renderSaveData(new Rect(num4 + num, num5, viewRect.width - num * 2f, num2), saveData, extraButtonStyle))
                {
                    GameManager.getInstance().setGameStateGameLoad(saveData.getPath(), saveData.getPlanetIndex(), saveData.createChallenge());
                }
                num5 += num2 + num;
            }
            GUI.EndScrollView();
            if (__instance.mGuiRenderer.renderBackButton(new Vector2(num4 + num3 * 0.5f, (float)Screen.height * 0.82f)))
            {
                GameManager.getInstance().setGameStateTitle();
            }
            return true;
        }
    }
}
