using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace RemoveTutorialFromMenu
{
    public class RemoveTutorialFromMenu : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new RemoveTutorialFromMenu(), modEntry, "RemoveTutorialFromMenu");

        public override void OnInitialized(ModEntry modEntry)
        {

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(GameStateTitle), "onGui")]
    public class MainMenuPatch
    {
        public static bool Prefix(GameStateTitle __instance)
        {
            Type instanceType = __instance.GetType();

            FieldInfo mGuiRendererInfo = Reflection.GetPrivateFieldOrThrow(instanceType, "mGuiRenderer", true);
            FieldInfo mAlphaInfo = Reflection.GetPrivateFieldOrThrow(instanceType, "mAlpha", true);
            FieldInfo mRightOffsetInfo = Reflection.GetPrivateFieldOrThrow(instanceType, "mRightOffset", true);
            FieldInfo mConfirmWindowInfo = Reflection.GetPrivateFieldOrThrow(instanceType, "mConfirmWindow", true);
            FieldInfo mAnySavegamesInfo = Reflection.GetPrivateFieldOrThrow(instanceType, "mAnySavegames", true);
            MethodInfo canAlreadyPlayInfo = Reflection.GetPrivateMethodOrThrow(instanceType, "canAlreadyPlay", true);
            MethodInfo renderTutorialRequestWindowInfo = Reflection.GetPrivateMethodOrThrow(instanceType, "renderTutorialRequestWindow", true);

            GuiRenderer mGuiRenderer = (GuiRenderer)Reflection.GetInstanceFieldValue(__instance, mGuiRendererInfo);

            if (mGuiRenderer == null)
            {
                mGuiRenderer = new GuiRenderer();
                Reflection.SetInstanceFieldValue(__instance, mGuiRendererInfo, mGuiRenderer);
            }
            ResourceList instance = ResourceList.getInstance();
            TitleTextures title = instance.Title;
            Texture2D gameTitle = title.GameTitle;
            Vector2 menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
            Vector2 titleLocation = Singleton<TitleScene>.getInstance().getTitleLocation();
            Vector2 menuLocation = Singleton<TitleScene>.getInstance().getMenuLocation();
            float num = (float)(Screen.height * gameTitle.height) / 1080f;
            float num2 = num * (float)gameTitle.width / (float)gameTitle.height;
            GUI.color = new Color(1f, 1f, 1f, (float)Reflection.GetInstanceFieldValue(__instance, mAlphaInfo));
            GUI.DrawTexture(new Rect(titleLocation.x - num2 * 0.5f, titleLocation.y, num2, num), gameTitle);
            GUI.color = Color.white;
            Texture2D backgroundRight = title.BackgroundRight;
            float num3 = (float)(Screen.height * backgroundRight.height) / 1080f;
            float num4 = num3 * (float)backgroundRight.width / (float)backgroundRight.height;
            GUI.DrawTexture(new Rect((float)Screen.width - num4 + (float)Reflection.GetInstanceFieldValue(__instance, mRightOffsetInfo), ((float)Screen.height - num3) * 0.75f, num4, num3), backgroundRight);
            float num5 = menuLocation.y * 0.95f;
            float num6 = menuButtonSize.y * 1.2f;
            menuLocation.x -= menuButtonSize.x;
            menuLocation.x += (float)Reflection.GetInstanceFieldValue(__instance, mRightOffsetInfo);
            if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("new_game"), FontSize.Huge, true))
            {
                GameManager.getInstance().setGameStateLocationSelection();
            }
            GUI.enabled = (bool)Reflection.GetInstanceFieldValue(__instance, mAnySavegamesInfo);
            num5 += num6;
            if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("continue_game"), FontSize.Huge, true))
            {
                GameManager.getInstance().setGameStateGameContinue();
            }
            num5 += num6;
            if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("load_game"), FontSize.Huge, true))
            {
                GameManager.getInstance().setGameStateLoadGame();
            }
            GUI.enabled = true;
            num5 += num6;
            if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("challenges"), FontSize.Huge, true))
            {
                GameManager.getInstance().setGameStateChallengeSelection();
            }
            num5 += num6;
            if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("settings"), FontSize.Huge, true))
            {
                GameManager.getInstance().setGameStateSettings();
            }
            num5 += num6;
            if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("quit"), FontSize.Huge, true))
            {
                Application.Quit();
            }
            if ((GuiConfirmWindow)Reflection.GetInstanceFieldValue(__instance, mConfirmWindowInfo) != null)
            {
                mGuiRenderer.renderWindow((GuiConfirmWindow)Reflection.GetInstanceFieldValue(__instance, mConfirmWindowInfo), null);
            }
            int num7 = 3;
            float num8 = menuButtonSize.y * 0.75f;
            float num9 = menuButtonSize.y * 0.25f;
            Vector2 vector = new Vector2(((float)Screen.width - (float)num7 * num8 - (float)(num7 - 1) * num9) * 0.5f, (float)Screen.height - num8 - num9 + (float)Reflection.GetInstanceFieldValue(__instance, mRightOffsetInfo) * 0.5f);
            Rect rect = new Rect(vector.x, vector.y, num8, num8);
            if (mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.Credits, StringList.get("credits")), null))
            {
                GameManager.getInstance().setGameStateCredits();
            }
            rect.x += num8 + num9;
            if (mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.SwitchPlanet, StringList.get("switch_planet")), null))
            {
                Singleton<TitleScene>.getInstance().switchPlanet();
            }
            rect.x += num8 + num9;

            return false;
        }
    }
    public static class Reflection
    {
        public static MethodInfo GetPrivateMethod(Type obj, string methodName, bool instance)
        {
            try
            {
                BindingFlags flags = BindingFlags.NonPublic | ((instance) ? BindingFlags.Instance : BindingFlags.Static);
                return obj.GetMethod(methodName, flags);
            }
            catch
            {
                return null;
            }
        }

        public static bool TryGetPrivateMethod(Type obj, string methodName, bool instance, out MethodInfo methodInfo)
        {
            methodInfo = GetPrivateMethod(obj, methodName, instance);
            return (methodInfo != null);
        }

        public static MethodInfo GetPrivateMethodOrThrow(Type obj, string methodName, bool instance)
        {
            MethodInfo methodInfo = GetPrivateMethod(obj, methodName, instance);
            if (methodInfo == null)
                throw new MissingMethodException($"Could not find \"{methodName}\"");

            return methodInfo;
        }

        public static FieldInfo GetPrivateField(Type obj, string fieldName, bool instance)
        {
            try
            {
                BindingFlags flags = BindingFlags.NonPublic | ((instance) ? BindingFlags.Instance : BindingFlags.Static);
                return obj.GetField(fieldName, flags);
            }
            catch
            {
                return null;
            }
        }

        public static FieldInfo GetPrivateFieldOrThrow(Type obj, string fieldName, bool instance)
        {
            FieldInfo fieldInfo = GetPrivateField(obj, fieldName, instance);
            if (fieldInfo == null)
                throw new MissingMethodException($"Could not find \"{fieldName}\"");

            return fieldInfo;
        }

        public static bool TryGetPrivateField(Type obj, string fieldName, bool instance, out FieldInfo fieldInfo)
        {
            fieldInfo = GetPrivateField(obj, fieldName, instance);
            return (fieldInfo != null);
        }

        public static object InvokeStaticMethod(MethodInfo method, params object[] args)
        {
            return method.Invoke(null, args);
        }

        public static object InvokeInstanceMethod(object instance, MethodInfo method, params object[] args)
        {
            return method.Invoke(instance, args);
        }

        public static object GetStaticFieldValue(FieldInfo field)
        {
            return field.GetValue(null);
        }

        public static object GetInstanceFieldValue(object instance, FieldInfo field)
        {
            return field.GetValue(instance);
        }

        public static void SetStaticFieldValue(FieldInfo field, object value)
        {
            field.SetValue(null, value);
        }

        public static void SetInstanceFieldValue(object instance, FieldInfo field, object value)
        {
            field.SetValue(instance, value);
        }
    }
}
