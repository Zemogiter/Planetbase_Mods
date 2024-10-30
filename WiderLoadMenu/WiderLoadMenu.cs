using System.IO;
using System.Reflection;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

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
    [HarmonyPatch(typeof(GameStateLoadGame), MethodType.Constructor)]
    public class ConstructorPatch
    {
        public static void Prefix(GameStateLoadGame __instance)
        {
            __instance.mRightOffset = (float)Screen.width * 10f;
            __instance.mSaveData = SaveData.loadAll();
            __instance.mScrollPosition = new Vector2(0f, 0f);
        }
    }
    [HarmonyPatch(typeof(GameStateLoadGame), nameof(GameStateLoadGame.renderSaveData))]
    public class RenderPatch
    {
        public static bool Prefix (GameStateLoadGame __instance, ref Rect rect, ref SaveData saveData)
        {
            float num = (float)GuiStyles.getIconMargin() * 0.25f; //changing this makes the buttons lose the round shape and become unclickable
            float num2 = rect.height - num * 2f; //no effect
            float num3 = 1.5f * num2; //stretches the preview images, making the rest of list entry (load saver button and remove save buttton) smaller
            float num4 = num2 * 0.5f; //increases remove save button on both axis, overlaps
            Rect rect2 = rect;
            rect2.width -= num4 + num * 6f + num3; //no effect
            rect2.x += num3 + num * 3f; //load save button grows horizontally
            rect2.y += num * 3f; //moves the load save button down
            rect2.height -= num * 6f; //shrinks the load save button
            bool flag = false;
            GUI.enabled = saveData.isValid();
            flag = __instance.mGuiRenderer.renderButton(rect2, saveData.getDescription());
            GUI.enabled = true;
            Rect position = new Rect(rect.x + num, rect.y + num, num3, num2);
            Texture2D screenshot = saveData.getScreenshot();
            if (screenshot != null)
            {
                GUI.DrawTexture(position, saveData.getScreenshot());
            }
            Rect rect3 = new Rect(rect.x + rect.width - num4 - num, rect.y + (rect.height - num4) * 0.5f, num4, num4); //changing 0.5f breaks the menu (moves the x button for every save downwards, making some of them unreachable)
            if (__instance.mGuiRenderer != null && __instance.mGuiRenderer.renderButton(rect3, ResourceList.StaticIcons.Cancel))
            {
                __instance.mSaveToDelete = saveData;
                __instance.mConfirmWindow = new GuiConfirmWindow(StringList.get("confirm_save_delete", Path.GetFileNameWithoutExtension(saveData.getName())), __instance.onDeleteSave);
                __instance.mConfirmWindow.setOnCancelCallback(__instance.onWindowCancel);
            }
            return flag;
        }
    }
    [HarmonyPatch(typeof(GameStateLoadGame), nameof(GameStateLoadGame.onGui))]
    public class OnGuiPatch
    {
        public static void Prefix(GameStateLoadGame __instance)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                return;
            }
            if (__instance.mConfirmWindow != null)
            {
                __instance.mGuiRenderer.renderWindow(__instance.mConfirmWindow, null);
                return;
            }
            float num = GuiStyles.getIconMargin();
            float num2 = (float)Screen.height * 0.1f; //does nothing
            float num3 = num2 * 7f; //makes a duplicate of the saves list behind the original one, streched across the screen
            float num4 = (float)Screen.width * 0.96f - num3 + __instance.mRightOffset; //this float does nothing, apparently
            float num5 = (float)Screen.height * 0.21f; //as above
            Rect rect = new Rect(num4, num5, num3, (num2 + num) * 5f); //entries in load menu are wider/doubled horizontaly
            Rect rect2 = rect;
            rect2.y -= num; //does nothing
            rect2.height += num * 2f; //similiar effect to rect 
            Rect viewRect = new Rect(num4 * 4f, num5 * 4f, num3 * 4f, (num2 + num) * (float)__instance.mSaveData.Count); //dosent seem to do anything
            string text = StringList.get("load_game");
            GUIStyle labelStyle = __instance.mGuiRenderer.getLabelStyle(FontSize.Huge, FontStyle.Bold, TextAnchor.MiddleCenter, FontType.Title);
            GUI.Label(new Rect(num4, (float)Screen.height * 0.06f, num3, (float)Screen.height * 0.2f), text, labelStyle); //first float creates a second "Load" string under the load menu entries, second one dosen't seem to do anything
            TitleTextures title = ResourceList.getInstance().Title;
            Texture2D backgroundLoadRight = title.BackgroundLoadRight;
            float num6 = (float)(Screen.height * backgroundLoadRight.height) / 1080f; //decreasing this float gives a green tint to the background
            float num7 = num6 * (float)backgroundLoadRight.width / (float)backgroundLoadRight.height; //adding a multiplayer stretches the outer "shell" of the load menu horizontally
            GUI.DrawTexture(new Rect((float)Screen.width - num7 + __instance.mRightOffset, ((float)Screen.height - num6) * 0.5f, num7, num6), backgroundLoadRight); //changign the float extends the elements of load menu downwards
            rect.width *= 1.05f; //dosent seem to do anything
            __instance.mScrollPosition = GUI.BeginScrollView(rect, __instance.mScrollPosition, viewRect);
            GUIStyle extraButtonStyle = Singleton<GuiStyles>.getInstance().getExtraButtonStyle(FontSize.Large, 6f);
            int count = __instance.mSaveData.Count;
            for (int i = 0; i < count; i++)
            {
                SaveData saveData = __instance.mSaveData[i];
                if (num5 + num2 > rect.y + __instance.mScrollPosition.y && num5 < rect.y + rect.height + __instance.mScrollPosition.y && __instance.renderSaveData(new Rect(num4 + num, num5, viewRect.width - num * 2f, num2), saveData, extraButtonStyle)) //changing the 2f makes another remove save button between the original one and load save button
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
