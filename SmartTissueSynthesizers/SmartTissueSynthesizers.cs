using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;

namespace SmartTissueSynthesizers
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Enable Bot Worshop portion of this mod")] public bool affectBotWorkshops = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }
    public class SmartTissueSynthesizers : ModBase
    {
        public static bool enabled;
        public static Settings settings;

        public static new void Init(ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            InitializeMod(new SmartTissueSynthesizers(), modEntry, "SmartTissueSynthesizers");
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

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
    }
    [HarmonyPatch(typeof(ConstructionComponent), nameof(ConstructionComponent.updateProduction))]
    public class TissueSynthesizerPatch
    {
        //main method
        static void Postfix(ConstructionComponent __instance)
        {
            //Tissue Synthesizer
            //To-do: find out why this crashes the game while placing tissue synthesizer
            List<ConstructionComponent> originalList = BuildableUtils.GetAllComponents();
            ComponentType tsType = TypeList<ComponentType, ComponentTypeList>.find<TissueSynthesizer>();
            List<ConstructionComponent> tsList = originalList.Where(a => a.getComponentType() == tsType).ToList();
            //var workshopType = BuildableUtils.FindComponentType<BotWorkshop>() as ComponentType;
            //var workshopList = originalList.Where(a => a.getComponentType() == workshopType).ToList();

            foreach(ConstructionComponent ts in tsList) 
            {
                if (__instance != null && __instance.isBuilt() && __instance.getResourceContainer() != null && __instance.getResourceContainer().contains(TypeList<ResourceType, ResourceTypeList>.find<Vitromeat>()) && __instance.isOperational() && __instance.isSelected() == false && __instance.isEnabled() && __instance.isSpaceAvailable())
                {
                    int currentIndex = __instance.getProducedItemIndex();
                    __instance.setProducedItemIndex(currentIndex + 1);
                    if (currentIndex > 3)
                    {
                        currentIndex = 0;
                    }
                }
                else
                {
                    return;
                }
            }
            //Bot Workshop
            /*if (SmartTissueSynthesizers.settings.affectBotWorkshops == true && __instance != null)
            {
                Specialization carrier = TypeList<Specialization, SpecializationList>.find<Carrier>();
                Specialization constructor = TypeList<Specialization, SpecializationList>.find<Constructor>();
                Specialization driller = TypeList<Specialization, SpecializationList>.find<Driller>();

                //a simple function that sets every existing bot workshop to a single production type, will probably remove once a function in else statement is ready
                //To-do: test it in the meantime because working on private m variables have proven to be diffilcut even with publicized assembly and unsafe mode
                foreach (ConstructionComponent workshop in workshopList)
                {
                    if (Character.getCountOfSpecialization(carrier) < ManufactureLimits.getInstance().getBotLimit(carrier).mValue)
                    {
                        __instance.setProducedItemIndex(1);
                    }
                    else if (Character.getCountOfSpecialization(constructor) < ManufactureLimits.getInstance().getBotLimit(constructor).mValue)
                    {
                        __instance.setProducedItemIndex(2);
                    }
                    else if (Character.getCountOfSpecialization(driller) < ManufactureLimits.getInstance().getBotLimit(driller).mValue)
                    {
                        __instance.setProducedItemIndex(3);
                    }
                    else
                    {
                        //To-do: add a functionality that sets a precentage of existing bot workshops to carrier, constructor and driller production, and if a group of workshops is done  they're redirected to other two

                    }
                }
            }*/
        }
    }

}
