using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
    public class CustomLoadMenu : GameStateLoadGame
    {
        public void GameStateLoadGame()
        {
            mRightOffset = (float)Screen.width;
            mSaveData = SaveData.loadAll();
            mScrollPosition = new Vector2(0f, 0f);
        }
        public override void onGui()
        {
            /*if (Input.GetKey(KeyCode.Space))
            {
                return;
            }*/
            if (mConfirmWindow != null)
            {
                mGuiRenderer.renderWindow(mConfirmWindow, null);
                return;
            }
            float num = GuiStyles.getIconMargin();
            float num2 = (float)Screen.height * 0.8f;
            float num3 = num2 * 7f;
            float num4 = (float)Screen.width * 2.5f - num3 + mRightOffset;
            float num5 = (float)Screen.height * 0.8f;
            Rect rect = new Rect(num4, num5, num3, (num2 + num) * 10f);
            Rect rect2 = rect;
            rect2.y -= num;
            rect2.height += num * 2f;
            Rect viewRect = new Rect(num4, num5, num3, (num2 + num) * (float)mSaveData.Count);
            string text = StringList.get("load_game");
            GUIStyle labelStyle = mGuiRenderer.getLabelStyle(FontSize.Huge, FontStyle.Bold, TextAnchor.MiddleCenter, FontType.Title);
            GUI.Label(new Rect(num4, (float)Screen.height * 1f, num3, (float)Screen.height * 0.9f), text, labelStyle);
            TitleTextures title = ResourceList.getInstance().Title;
            Texture2D backgroundLoadRight = title.BackgroundLoadRight;
            float num6 = (float)(Screen.height * backgroundLoadRight.height) / 1080f;
            float num7 = num6 * (float)backgroundLoadRight.width / (float)backgroundLoadRight.height;
            GUI.DrawTexture(new Rect((float)Screen.width - num7 + mRightOffset, ((float)Screen.height - num6) * 1f, num7, num6), backgroundLoadRight);
            rect.width *= 2.1f;
            mScrollPosition = GUI.BeginScrollView(rect, mScrollPosition, viewRect);
            GUIStyle extraButtonStyle = Singleton<GuiStyles>.getInstance().getExtraButtonStyle(FontSize.Large, 8f);
            int count = mSaveData.Count;
            for (int i = 0; i < count; i++)
            {
                SaveData saveData = mSaveData[i];
                if (num5 + num2 > rect.y + mScrollPosition.y && num5 < rect.y + rect.height + mScrollPosition.y && renderSaveData(new Rect(num4 + num, num5, viewRect.width - num * 4f, num2), saveData, extraButtonStyle))
                {
                    GameManager.getInstance().setGameStateGameLoad(saveData.getPath(), saveData.getPlanetIndex(), saveData.createChallenge());
                }
                num5 += num2 + num;
            }
            GUI.EndScrollView();
            if (mGuiRenderer.renderBackButton(new Vector2(num4 + num3 * 0.8f, (float)Screen.height * 1.2f)))
            {
                GameManager.getInstance().setGameStateTitle();
            }
        }
        public override void update(float timeStep)
        {
            base.update(timeStep);

            Singleton<TitleScene>.getInstance().updateOffset(ref mRightOffset);
        }
    }
}
