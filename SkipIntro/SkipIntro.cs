using Planetbase;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;
using System;

namespace SkipIntro
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Skip intro Button")] public KeyCode SkipIntroButton = KeyCode.Escape;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class SkipIntro : ModBase
    {
        IntroCinemetic m_intro;
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new SkipIntro(), modEntry, "SkipIntro");
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
            m_intro = null;
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            //main code
            //to-do: fix an issue that prevents going to the pause menu while the ship is landing and it's passangers are getting off
            if(GameManager.getInstance().getGameState() is GameStateGame gameStateGame)
            {
                if (m_intro == null)
                {
                    m_intro = CameraManager.getInstance().getCinematic() as IntroCinemetic;
                    if (m_intro == null)
                    {
                        return;
                    }
                }
                ColonyShip colonyShip = CoreUtils.GetMember<IntroCinemetic, ColonyShip>("mColonyShip", m_intro);
                Console.WriteLine("SkipIntro - ColonyShip: " + colonyShip);
                if (colonyShip.isDone())
                {
                    m_intro = null;
                    return;
                }

                GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
                Console.WriteLine("SkipIntro - GameState: " + gameState);
                var gameGui = CoreUtils.GetMember<GameStateGame, GameGui>("mGameGui", gameState);
                Console.WriteLine("SkipIntro - GameGui: " + gameGui);
                Console.WriteLine("SkipIntro - GameGui Window: " + gameGui.getWindow());

                if (gameGui.getWindow() == null)
                {
                    // Set a valid GuiWindow instance
                    gameGui.setWindow(new GuiGameMenu());
                    Console.WriteLine("SkipIntro - GameGui Window after setting: " + gameGui.getWindow());
                }

                if (gameGui.getWindow() is GuiGameMenu)
                {
                    gameGui.setWindow(null);
                    Console.WriteLine("SkipIntro - GameGui Window: " + gameGui.getWindow());
                }

                if (Input.GetKeyDown(KeyCode.Escape) && CameraManager.getInstance().getCinematic() != null)
                {
                    Console.WriteLine("SkipIntro - Escape key pressed and we're in a cinematic");
                    PhysicsUtil.findFloor(colonyShip.getPosition(), out Vector3 shipLandingPosition, 256);
                    shipLandingPosition.y = CameraManager.DefaultHeight;
                    Transform transform = CameraManager.getInstance().getTransform();
                    transform.position = shipLandingPosition + colonyShip.getDirection().flatDirection() * 50f;
                    transform.LookAt(shipLandingPosition);
                    Vector3 eulerAngles = transform.eulerAngles;
                    eulerAngles.x = 25f; //it's the same as VerticalAngles property in CameraManager
                    transform.rotation = Quaternion.Euler(eulerAngles);
                    CoreUtils.SetMember<IntroCinemetic, float>("mBlackBars", m_intro, 0f);
                    CameraManager.getInstance().setCinematic(null);
                }
            }
        }
    }
}
