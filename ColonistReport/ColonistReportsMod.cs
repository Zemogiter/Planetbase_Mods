using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace ColonistReport
{
    public class ColonistReportsMod : ModBase
    {
        static GuiReportsMenuItem ReportsMenuItem { get; set; } 
        static GuiReportsMenu ReportsMenu { get; set; }

        public static new void Init(ModEntry modEntry)
        {
            WorkloadManager.mInstance = new WorkloadManager();

            ReportsMenu = new GuiReportsMenu();
            InitializeMod(new ColonistReportsMod(), modEntry, "Colonist Reports Mod");
        }

        public void OnGameStart()
        {
            if (GuiMenuSystem is GuiMenu menuBaseManagement)
            {
                if (!menuBaseManagement.mItems.Contains(ReportsMenuItem))
                {
                    var insertIndex = menuBaseManagement.mBackItem == null ?
                        menuBaseManagement.getItemCount() - 1 :
                        menuBaseManagement.mItems.IndexOf(menuBaseManagement.mBackItem);

                    menuBaseManagement.mItems.Insert(insertIndex, ReportsMenuItem);
                }
            }
        }

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
            ReportsMenuItem = new GuiReportsMenuItem(new GuiDefinitions.Callback(OnReportsMenuOpen));
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            WorkloadManager.getInstance().Update(timeStep);
            if (GameStateGame.mGameGui.getWindow() is GuiReportsMenu menu)
            {
                menu.updateUi();
            }
        }

        private void RegisterStrings()
        {
            StringUtils.RegisterString("reports", "Base Reports");
            StringUtils.RegisterString("reports_workload", "Colonist Workload");
            StringUtils.RegisterString("reports_worker_workload", "Worker Workload");
        }

        private void OnReportsMenuOpen(object parameter)
        {
            if (GameStateGame.mGameGui.getWindow() is GuiReportsMenu)
            {
                GameStateGame.mGameGui.setWindow(null);
            }
            else
            {
                GameStateGame.mGameGui.setWindow(ReportsMenu);
            }
        }
    }
}
