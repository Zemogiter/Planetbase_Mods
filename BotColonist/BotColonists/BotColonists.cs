using System.IO;
using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;
using HarmonyLib;
using ResourceType = Planetbase.ResourceType;
using Resource = Planetbase.Resource;
using System.Collections.Generic;

namespace BotColonists
{
    public class BotColonists : ModBase
    {
        public static int mNumbers;

        public static int vistors;

        public static int mResources;

        public static int mBots;

        public static new void Init(ModEntry modEntry) => InitializeMod(new BotColonists(), modEntry, "BotColonists");

        public override void OnInitialized(ModEntry modEntry)
        {
            string path = "./Mods/BotColonists/Numbers.txt";
            string path2 = "./Mods/BotColonists/Bots.txt";
            string path3 = "./Mods/BotColonists/Resources.txt";
            mNumbers = int.Parse(((string)(object)new StreamReader(path).ReadLine()).Substring(0));
            mBots = int.Parse(((string)(object)new StreamReader(path2).ReadLine()).Substring(0));
            mResources = int.Parse(((string)(object)new StreamReader(path3).ReadLine()).Substring(0));
            Debug.Log("[MOD]BotColonists Loaded succesfully");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
        }

        static BotColonists()
        {
            vistors = 20;
        }
    }
    [HarmonyPatch(typeof(ColonistShip), nameof(ColonistShip.onLanded))]
    public class ColonistShipPatch
    {
        
        public static bool Prefix(ColonistShip __instance)
        {
            
            var sizeGet = CoreUtils.GetMember<LandingShip, LandingShip.Size>("mSize");
            var intrudersGet = CoreUtils.GetMember<LandingShip, bool>("mIntruders");
            var spawnPositionGet = CoreUtils.
            Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
            int num = 10;
            int num2 = 10;
            int num3 = 10;
            if (BotColonists.mNumbers != 0)
            {
                num = BotColonists.mNumbers;
            }
            if (BotColonists.mBots != 0)
            {
                num2 = BotColonists.mBots;
            }
            if (BotColonists.mResources != 0)
            {
                num3 = BotColonists.mResources;
            }
            if ((float)num > 0.9f)
            {
                num += Random.Range(2, 4);
            }
            else if ((float)num > 0.7f)
            {
                BotColonists.mNumbers += Random.Range(1, 3);
            }
            if (sizeGet == LandingShip.Size.Large)
            {
                BotColonists.mNumbers *= 2;
            }
            if (intrudersGet)
            {
                num += LandingShipManager.getExtraIntruders();
            }
            for (int i = 0; i < num; i++)
            {
                var landingShip = LandingShip.find(i).GetType();
                Specialization specialization = ((!intrudersGet) ? __instance.calculateSpecialization() : TypeList<Specialization, SpecializationList>.find<Intruder>());
                if (specialization != null)
                {
                    Character.create(specialization, LandingShip.getSpawnPosition(i), Location.Exterior);
                }
            }
            for (int j = 0; j < num2; j++)
            {
                Specialization specialization2 = TypeList<Specialization, SpecializationList>.find<Carrier>();
                Specialization specialization3 = TypeList<Specialization, SpecializationList>.find<Constructor>();
                Specialization specialization4 = TypeList<Specialization, SpecializationList>.find<Driller>();
                Character.create(specialization2, LandingShip.getSpawnPosition(j), Location.Exterior);
                Character.create(specialization3, LandingShip.getSpawnPosition(j), Location.Exterior);
                Character.create(specialization4, LandingShip.getSpawnPosition(j), Location.Exterior);
            }
            for (int k = 0; k < num3; k++)
            {
                Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Meal>(), LandingShip.getSpawnPosition(k), Location.Exterior);
                Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), LandingShip.getSpawnPosition(k), Location.Exterior);
                Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), LandingShip.getSpawnPosition(k), Location.Exterior);
                Resource.create(TypeList<ResourceType, ResourceTypeList>.find<MedicalSupplies>(), LandingShip.getSpawnPosition(k), Location.Exterior);
                Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Spares>(), LandingShip.getSpawnPosition(k), Location.Exterior);
            }

            return false;
        }
    }
    public class StaticLanding : LandingShip
    {
        public override GameObject getPrefab()
        {
            throw new System.NotImplementedException();
        }
    }
}