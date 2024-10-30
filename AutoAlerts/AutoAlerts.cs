using System.Collections.Generic;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace AutoAlerts
{
    public class AutoAlerts : ModBase
    {
        private bool m_autoActivated;
        private AlertState m_activatedState;

        public static new void Init(ModEntry modEntry) => InitializeMod(new AutoAlerts(), modEntry, "AutoAlerts");

		public override void OnInitialized(ModEntry modEntry)
		{
            m_activatedState = AlertState.NoAlert;
            m_autoActivated = false;

            Debug.Log("[MOD] AutoAlerts activated");
        }

		public override void OnUpdate(ModEntry modEntry, float timeStep)
		{
            if (ConstructionComponent.findOperational(TypeList<ComponentType, ComponentTypeList>.find<SecurityConsole>()) == null) //if no functional Security Console component is in any Control Room on map, the mod does nothing
                return;

            AlertState state = SecurityManager.getInstance().getAlertState();

            // if the state has been changed manually, don't do anything else. Will be activated again if the player sets NoAlert
            if (state != m_activatedState)
            {
                m_activatedState = AlertState.NoAlert;
                m_autoActivated = false;
                return;
            }

            List<Character> intruders = Character.getSpecializationCharacters(SpecializationList.IntruderInstance);
            if (intruders != null)
            {
                foreach (Character intruder in intruders)
                {
                    if (intruder.hasStatusFlag(Character.StatusFlagDetected))
                    {
                        // check number of guards vs intruders - want to keep on yellow while ratio guards/intruders is high enough
                        float numIntruders = intruders.Count;
                        float numGuards = Character.getCountOfSpecialization(TypeList<Specialization, SpecializationList>.find<Guard>());

                        float ratio = numGuards / numIntruders;
                        AlertState newState = ratio < 0.75f ? AlertState.RedAlert : AlertState.YellowAlert;

                        if (newState != m_activatedState)
                        {
                            SecurityManager.getInstance().setAlertState(newState);
                            m_activatedState = newState;
                            m_autoActivated = true;
                        }

                        return;
                    }
                }
            }

            if (DisasterManager.getInstance().anyInProgress())
            {
                if (state != AlertState.YellowAlert)
                {
                    SecurityManager.getInstance().setAlertState(AlertState.YellowAlert);
                    m_activatedState = AlertState.YellowAlert;
                    m_autoActivated = true;
                }

                return;
            }

            if (m_autoActivated)
            {
                // Only disable alert if it's the same one we set
                if (state == m_activatedState)
                    SecurityManager.getInstance().setAlertState(AlertState.NoAlert);

                m_activatedState = AlertState.NoAlert;
                m_autoActivated = false;
            }
        }
    }
}
