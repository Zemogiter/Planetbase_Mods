using Planetbase;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;

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
        private IntroCinemetic m_intro;
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
            if (m_intro == null)
            {
                m_intro = CameraManager.getInstance().getCinematic() as IntroCinemetic;
                if (m_intro == null)
                {
                    return;
                }
            }
            ColonyShip colonyShip = ColonyShip.getFirstOfType<ColonyShip>();
            if (colonyShip.isDone())
            {
                m_intro = null;
                return;
            }

            GameStateGame gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            var instance = MenuUtils.GetMenuSystem(gameStateGame);
            if (MenuUtils.GetMenu(instance,gameStateGame.ToString()) is GuiGameMenu) //this line needs to be changes
		    {
                MenuUtils.SetMenu(gameStateGame.GetMenuSystem(), null, null);
                //gameStateGame.mGameGui.setWindow(null);
            }
            
            if (InputAction.isValidKey(SkipIntro.settings.SkipIntroButton) && CameraManager.getInstance().getCinematic() != null && gameStateGame.inTutorial() == false)
            {
                PhysicsUtil.findFloor(colonyShip.getPosition(), out var terrainPosition);
                terrainPosition.y += 21f;
                Transform transform = CameraManager.getInstance().getTransform();
                transform.position = terrainPosition + colonyShip.getDirection().flatDirection() * 50f;
                transform.LookAt(terrainPosition);
                Vector3 eulerAngles = transform.eulerAngles;
                eulerAngles.x = 25f;
                transform.rotation = Quaternion.Euler(eulerAngles);
                //m_intro.mBlackBars = 0f;
                CameraManager.getInstance().setCinematic(null);
            }
        }
    }
}
