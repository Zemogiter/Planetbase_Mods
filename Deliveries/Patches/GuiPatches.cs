using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Linq;
using UnityEngine;

namespace Deliveries.Patches
{
    /// <summary>
    /// Add the deliver ship button to the main menu bar
    /// </summary>

    [HarmonyPatch(typeof(global::Planetbase.GuiMenuSystem))]
    [HarmonyPatch("init")]
    public class InitPatch
    {
        public static void Postfix(global::Planetbase.GuiMenuSystem __instance)
        {
            var buttonIcon = ResourceUtil.loadIconColor("Ships/icon_ship_colony");
            //Create the new button
            var deliveryButton = new GuiMenuItem(
                buttonIcon,
                StringList.get("menu_deliver"),
                parameter =>
                {
                    if (Deliveries.ActiveDeliveryShip) return; //if there is already a delivery ship active, dont do anything)
                    if(BuildableUtils.GetAllModules().FirstOrDefault((Module module) => module.getName() == ModuleName.LandingPad.ToString() || module.getName() == ModuleName.Starport.ToString()) == null) //checking if we have Landing Pad/Starport on map
                    {
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_error", Deliveries.DeliveryErrorMessage), buttonIcon, 2));
                        return;
                    }
                    var colonyShipResourcesValue = PlanetManager.getCurrentPlanet().getStartingResources().getValue();
                    var colonyCoins = ResourceTypeList.CoinsInstance.getValue();
                    if(Deliveries.settings.cheatMode == false && colonyCoins < colonyShipResourcesValue) //checking if the player has enough coins
                    {
                        Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_error2", Deliveries.DeliveryErrorMessage2), buttonIcon, 2));
                        return;
                    }
                    Deliveries.ActiveDeliveryShip = true;

                    var landingObject = BuildableUtils.GetAllModules().FirstOrDefault((Module module) => module.getName() == ModuleName.LandingPad.ToString() || module.getName() == ModuleName.Starport.ToString());
                    var startPosition = landingObject.getPosition();
                    Deliveries.Ship = global::Planetbase.ColonyShip.create(
                        startPosition + Vector3.up * 100f,
                        startPosition,
                        PlanetManager.getCurrentPlanet().getStartingResources()
                    );
                }
            );

            //Add the new button to the main GUI menu and reorder it
            //var mainMenu = __instance.mMenuMain;

            //mainMenu.mItems.Remove(__instance.mItemHelp);
            //mainMenu.addItem(pauseMenuItem);
            //mainMenu.addItem(__instance.mItemHelp);
            GuiMenu member = CoreUtils.GetMember<GuiMenuSystem, GuiMenu>("mMenuBaseManagement", __instance);
            member.AddItemBeforeBackItem(deliveryButton);
            member.
        }
    }
}
