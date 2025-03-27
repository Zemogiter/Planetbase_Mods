using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Deliveries.Patches
{
    /*public class CustomGameStateGame : GameStateGame
    {
        public CustomGameStateGame(string file, int planetIndex, Challenge challenge) : base(file, planetIndex, challenge)
        {
        }
        public void onButtonDelivery()
        {

        }
    }
    [HarmonyPatch(typeof(GameStateGame), nameof(GameStateGame.update))]
    public class KeybindPatch
    {
        public static void Postfix(GameStateGame __instance)
        {
            if (InputAction.isValidKey(Deliveries.settings.DeliveryKey))
            {
                //add logic that activates the button in GuiPatches
                Patches.InitPatch.Postfix(null);
            }
        }
    }*/
}
