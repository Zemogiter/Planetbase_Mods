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
    
    public class ButtonContainer
    {
        public static Texture2D ButtonIconFunction()
        {
            return ResourceUtil.loadIconColor("Ships/icon_ship_colony");
        }
        public static GuiDefinitions.Callback buttonFunctions = parameter =>
        {
            //obtaining the necessary variables to be used in the new button
            //to-do: verify that buildingCheck2 is working
            var landingpad = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeLandingPad>();
            var starport = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStarport>();
            var buildingCheck = BuildableUtils.GetAllModules().FirstOrDefault((Module module) => module.getModuleType() == landingpad || module.getModuleType() == starport);
            var buildingCheck2 = BuildableUtils.GetAllModules().FirstOrDefault((Module module) => buildingCheck.anyTargeters() == false);
            int colonyShipResourcesValue = PlanetManager.getCurrentPlanet().getStartingResources().getValue();
            //to-do: need to find out a way to get the player's current coins
            //int colonyCoins = Resource.find(ResourceType.FlagCurrency).get;
            if (Deliveries.ActiveDeliveryShip) return; //if there is already a delivery ship active, dont do anything)
            if (buildingCheck == null) //checking if we have Landing Pad/Starport on map
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_error", Deliveries.DeliveryErrorMessage), ButtonIconFunction(), 1));
                return;
            }
            if (Deliveries.settings.cheatMode == false && colonyCoins < colonyShipResourcesValue) //checking if the player has enough coins
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message("Not enough coins to dispatch delivery. Coins needed: " + colonyShipResourcesValue.ToString(), ButtonIconFunction(), 1));
                return;
            }
            if (buildingCheck2 == null) //checking if the Landing Pad/Starport is occupied
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_error2", Deliveries.DeliveryErrorMessage2), ButtonIconFunction(), 1));
                return;
            }
            Deliveries.ActiveDeliveryShip = true;

            if (Deliveries.settings.cheatMode == false)
            {
                //to-do: add a function to remove the coins from the player's inventory after the delivery ship is dispatched
                int coinsAfterDelivery = colonyCoins - colonyShipResourcesValue;
            }
            var startPosition = buildingCheck.getPosition();
            Deliveries.Ship = global::Planetbase.ColonyShip.create(
                startPosition + Vector3.up * 100f,
                startPosition,
                PlanetManager.getCurrentPlanet().getStartingResources()
            );
            if (Deliveries.settings.cheatMode == true)
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_cheater"), ButtonIconFunction(), 1));
            }
        };
    }
    [HarmonyPatch(typeof(GuiMenuSystem))]
    [HarmonyPatch("init")]
    public class InitPatch
    {
        public static void Postfix(GuiMenuSystem __instance)
        {
            //Create the new button
            var deliveryButton = new GuiMenuItem(
                ButtonContainer.ButtonIconFunction(),
                StringList.get("menu_deliver"), ButtonContainer.buttonFunctions
            );

            //Add the new button to the main GUI menu and reorder it
            GuiMenu member = CoreUtils.GetMember<GuiMenuSystem, GuiMenu>("mMenuBaseManagement", __instance);
            member.AddItemBeforeBackItem(deliveryButton);
        }
    }
    public class KeybindPatch
    {
        public static void Postfix(GuiMenuSystem __instance)
        {
            //to-do: find a way to call this code without copy-pasting it
            if (InputAction.isValidKey(Deliveries.settings.DeliveryKey))
            {
                
            }
        }
    }
}
