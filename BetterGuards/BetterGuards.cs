using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterGuards
{
    public class BetterGuards : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new BetterGuards(), modEntry, "BetterGuards");

        public static float healthmult;
        public override void OnInitialized(ModEntry modEntry)
        {
            var path = "./Mods/BetterGuards/config.txt";
            string line;
            System.IO.StreamReader file = new(path);
            line = file.ReadLine();
            line = line.Substring(14);
            healthmult = float.Parse(line);
            Console.WriteLine("The value of healthmult is " + healthmult);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

            List<Character> guards = Character.getSpecializationCharacters(SpecializationList.findPartial("Guard"));
            foreach (Character guard in guards)
            {
                
                //make guards drop guns on death (vanilla = guns disapear with guards)
                if (guard.isDead() == true)
                {
                    var transform = new NewSelectable();
                    
                    Vector3 position = guard.getPosition();
                    Location location = guard.getLocation();
                    ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();

                    Resource droppedGun = Resource.create(gunType, position, location);
                    droppedGun.setRotation(transform.getTransform().rotation);
                    droppedGun.drop(Resource.State.Idle);
                }
            }
            List<Character> intruders = Character.getSpecializationCharacters(SpecializationList.findPartial("Intruder"));
            foreach(Character intruder in intruders)
            {
                if(intruder.isDead() == true)
                {
                    var transform = new NewSelectable();

                    Vector3 position = intruder.getPosition();
                    Location location = intruder.getLocation();
                    ResourceType gunType = TypeList<ResourceType, ResourceTypeList>.find<Gun>();

                    Resource droppedGun = Resource.create(gunType, position, location);
                    droppedGun.setRotation(transform.getTransform().rotation);
                    droppedGun.drop(Resource.State.Idle);
                }
            }
        }
    }
    public class NewSelectable : Selectable
    {
        public override GameObject getGameObject()
        {
            throw new NotImplementedException();
        }

        public override float getHeight()
        {
            throw new NotImplementedException();
        }

        public override Texture2D getIcon()
        {
            throw new NotImplementedException();
        }

        public override int getId()
        {
            throw new NotImplementedException();
        }

        public override Location getLocation()
        {
            throw new NotImplementedException();
        }

        public override string getName()
        {
            throw new NotImplementedException();
        }

        public override Vector3 getPosition()
        {
            throw new NotImplementedException();
        }

        public override float getRadius()
        {
            throw new NotImplementedException();
        }

        public override GameObject getSelectionModel()
        {
            throw new NotImplementedException();
        }
    }
}
