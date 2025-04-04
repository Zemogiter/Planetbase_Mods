using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
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
            int colonyShipResourcesValue = PlanetManager.getCurrentPlanet().getStartingResources().getValue();
            int colonyCoins = Resource.getCountOfType(TypeList<ResourceType, ResourceTypeList>.find<Coins>());

            if (Deliveries.ActiveDeliveryShip) return; //if there is already a delivery ship active, dont do anything)
            if (LandingCheck() == null) //checking if we have free Landing Pad/Starport on map
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_error", Deliveries.DeliveryErrorMessage), ButtonIconFunction(), 1));
                return;
            }
            if (Deliveries.settings.cheatMode == false && colonyCoins < colonyShipResourcesValue) //checking if the player has enough coins
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message("Not enough coins for delivery. Need: " + colonyShipResourcesValue.ToString() + " but we have: " + colonyCoins, ButtonIconFunction(), 1));
                return;
            }
            Deliveries.ActiveDeliveryShip = true;

            if (Deliveries.settings.cheatMode == false)
            {
                int coinsAfterDelivery = colonyCoins - colonyShipResourcesValue;
                if (Deliveries.settings.debugMode) Console.WriteLine("Coins after delivery: " + coinsAfterDelivery);
                ResourceAmount resourceAmount = new ResourceAmount(TypeList<ResourceType, ResourceTypeList>.find<Coins>(), colonyShipResourcesValue);
                if (Deliveries.settings.debugMode) Console.WriteLine("The value of resourceAmount: " + resourceAmount);
                Resource.removeInmaterialResource(resourceAmount); //removing the coins from the player, needs further testing

            }
            Vector3 startPosition = LandingCheck().getPosition();
            if (startPosition != null)
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_error", Deliveries.DeliveryErrorMessage), ButtonIconFunction(), 1));
                return;
            } 
            if (Deliveries.settings.debugMode) Console.WriteLine("Deliveries - The value of startPosition: " + startPosition);
            if (Deliveries.settings.debugMode) Console.WriteLine("Deliveries - Colony ship average position: " + ColonyShip.getAveragePosition());
            if (startPosition == null) return;
            if (startPosition == ColonyShip.getAveragePosition())
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message("Coding error prevents getting the correct landing location. Contact the developers!",ButtonIconFunction(), 1));
                return;
            }

            //to-do: figure out why this is spawing the ship at the original spawn point instead of the landing pad/starport
            Deliveries.Ship = ColonyShip.create(
                startPosition + Vector3.up * 100f,
                startPosition,
                PlanetManager.getCurrentPlanet().getStartingResources()
            );
            if (Deliveries.settings.cheatMode == true)
            {
                Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("delivery_cheater"), ButtonIconFunction(), 1));
            }
        };
        public static Module LandingCheck() //checks for a Landing Pad/Starport that is empty and returns it
        {
            var landingpad = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeLandingPad>();
            var starport = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStarport>();
            Module landingZone = BuildableUtils.GetAllModules().FirstOrDefault((Module module) => module.getModuleType() == landingpad || module.getModuleType() == starport);
            if (Deliveries.settings.debugMode) Console.WriteLine("Deliveries - Initially choosen landing faility: " + landingZone.getName() + landingZone.getLocation());
            Module freeLandingZone = BuildableUtils.GetAllModules().FirstOrDefault(module => (module.getModuleType() == landingpad || module.getModuleType() == starport) && module.isTargeter(landingZone) == false);
            if (Deliveries.settings.debugMode) Console.WriteLine("Deliveries - Free landing facility: " + freeLandingZone.getName() + freeLandingZone.getLocation());

            if (freeLandingZone == null)
            {
                return null;
            }
            return freeLandingZone;
        }
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
            if (Input.GetKeyDown(Deliveries.settings.DeliveryKey))
            {
                
            }
        }
    }
}
