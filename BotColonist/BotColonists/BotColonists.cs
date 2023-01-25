using System.IO;
using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;

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
            string path = "./Mods/BotColonist/Numbers.txt";
            string path2 = "./Mods/BotColonist/Bots.txt";
            string path3 = "./Mods/BotColonist/Resources.txt";
            mNumbers = int.Parse(((string)(object)new StreamReader(path).ReadLine()).Substring(0));
            mBots = int.Parse(((string)(object)new StreamReader(path2).ReadLine()).Substring(0));
            mResources = int.Parse(((string)(object)new StreamReader(path3).ReadLine()).Substring(0));
            Debug.Log("[MOD]BotColonists Loaded succesfully");
            var custom = new CustomColonistShip();
            custom.onLanded();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            
        }
        public class CustomColonistShip : ColonistShip
        {
#pragma warning disable IDE1006 // Naming Styles
            public new void onLanded()
#pragma warning restore IDE1006 // Naming Styles
            {
                Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
                int num = 10;
                int num2 = 10;
                int num3 = 10;
                if (mNumbers != 0)
                {
                    num = mNumbers;
                }
                if (mBots != 0)
                {
                    num2 = mBots;
                }
                if (mResources != 0)
                {
                    num3 = mResources;
                }
                if ((float)num > 0.9f)
                {
                    num += Random.Range(2, 4);
                }
                else if ((float)num > 0.7f)
                {
                    mNumbers += Random.Range(1, 3);
                }
                if (mSize == Size.Large)
                {
                    mNumbers *= 2;
                }
                if (mIntruders)
                {
                    num += LandingShipManager.getExtraIntruders();
                }
                for (int i = 0; i < num; i++)
                {
                    Specialization specialization = ((!mIntruders) ? base.calculateSpecialization() : TypeList<Specialization, SpecializationList>.find<Intruder>());
                    if (specialization != null)
                    {
                        Character.create(specialization, getSpawnPosition(i), Location.Exterior);
                    }
                }
                for (int j = 0; j < num2; j++)
                {
                    Specialization specialization2 = TypeList<Specialization, SpecializationList>.find<Carrier>();
                    Specialization specialization3 = TypeList<Specialization, SpecializationList>.find<Constructor>();
                    Specialization specialization4 = TypeList<Specialization, SpecializationList>.find<Driller>();
                    Character.create(specialization2, getSpawnPosition(j), Location.Exterior);
                    Character.create(specialization3, getSpawnPosition(j), Location.Exterior);
                    Character.create(specialization4, getSpawnPosition(j), Location.Exterior);
                }
                for (int k = 0; k < num3; k++)
                {
                    Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Meal>(), getSpawnPosition(k), Location.Exterior);
                    Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Metal>(), getSpawnPosition(k), Location.Exterior);
                    Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>(), getSpawnPosition(k), Location.Exterior);
                    Resource.create(TypeList<ResourceType, ResourceTypeList>.find<MedicalSupplies>(), getSpawnPosition(k), Location.Exterior);
                    Resource.create(TypeList<ResourceType, ResourceTypeList>.find<Spares>(), getSpawnPosition(k), Location.Exterior);
                }
            }
        }

        static BotColonists()
        {
            vistors = 20;
        }
    }
}