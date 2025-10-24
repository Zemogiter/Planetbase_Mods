using System;
using System.Reflection;
using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace CharacterCam
{
    public class CharacterCam : ModBase
    {
        public const string MESSAGE = "Disengaging character camera from visitor.";
        public new static void Init(ModEntry modEntry) => InitializeMod(new CharacterCam(), modEntry, "CharacterCam");

        public override void OnInitialized(ModEntry modEntry)
		{
            RegisterStrings();
            System.Diagnostics.Debug.WriteLine("[MOD] CharacterCam activated");
        }

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            
        }
        private static void RegisterStrings()
        {
            StringUtils.RegisterString("message_disengaging_visitor_cam", MESSAGE);
            StringUtils.RegisterString("CharacterCam", "Character Cam");
        }
    }
    [HarmonyPatch(typeof(CloseCameraCinematic), nameof(CloseCameraCinematic.updateCharacter))]
    public class CloseCameraCinematicPatch
    {
        public static bool Prefix(Character character, float timeStep)
        {
            
            Transform cameraTransform = CameraManager.getInstance().getTransform();
            Transform characterTransform = character.getTransform();

            double yAngle = characterTransform.eulerAngles.y;
            FieldInfo lastRotation = typeof(CloseCameraCinematic).GetField("mLastRotation");
            double fiC = Convert.ToDouble(lastRotation);
            Debug.Log(fiC);
            float horizontalBobbing = Mathf.Clamp((float)((Convert.ToDouble(lastRotation) - yAngle) * 0.25f), -0.5f, 0.5f);
            Vector3 newPos = characterTransform.position + Vector3.up * character.getHeight() + characterTransform.forward * 0.7f + horizontalBobbing * characterTransform.right;
            FieldInfo fi2 = typeof(CloseCameraCinematic).GetField("mFirstUpdate");
            Debug.Log(fi2);
            if (Convert.ToBoolean(fi2))
            {
                cameraTransform.position = newPos;
                cameraTransform.rotation = characterTransform.rotation;
                fiC = yAngle;
                Debug.Log(fiC);
            }
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, newPos, 0.1f);
            Vector3 lookAtDir = (characterTransform.position + characterTransform.forward * 1.4f + Vector3.up * (character.getHeight() * 0.85f) - cameraTransform.position).normalized;
            cameraTransform.rotation = Quaternion.RotateTowards(cameraTransform.rotation, Quaternion.LookRotation(lookAtDir), timeStep * 120f);

            return false;
        }
    }
    //a patch to make the camera disengage from visitors upon returning to the ship, otherwise the game will crash
    [HarmonyPatch(typeof(AiRuleGoBackToShip), nameof(AiRuleGoBackToShip.update))]
    public class DisengageVisitorCamera
    {
        public static void Postfix(Character __instance)
        {
            if (__instance.isSelected() && __instance.getSpecialization() == SpecializationList.VisitorInstance)
            {
                var i2 = GameManager.getInstance().getGameState() as GameStateGame;
                i2.endCloseCamera();
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_disengaging_visitor_cam", CharacterCam.MESSAGE), ResourceList.StaticIcons.Visitor, 1));
            }
        }
    }
}
