using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarchlessPlants
{
    public class StarchlessPlants : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new StarchlessPlants(), modEntry, "StarchlessPlants");

        public const string Description = "Same food output as default pad, but without starch. Usefull if you need more vegetables but not starch.";

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
            RegisterNewComponents();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {

        }
        private static void RegisterStrings()
        {
            StarchlessPeaPad.RegisterStrings();
            StarchlessRicePad.RegisterStrings();
            StarchlessPotatoPad.RegisterStrings();
            StarchlessWheatPad.RegisterStrings();
            StarchlessMaizePad.RegisterStrings();
        }
        private void RegisterNewComponents()
        {
            // Register new components to global lists
            var componentTypeList = ComponentTypeList.getInstance();

            componentTypeList.AddType(new StarchlessPeaPad());
            componentTypeList.AddType(new StarchlessRicePad());
            componentTypeList.AddType(new StarchlessPotatoPad());
            componentTypeList.AddType(new StarchlessWheatPad());
            componentTypeList.AddType(new StarchlessMaizePad());

            // Add new components to bio domes
            var bioDomeComponents = BuildableUtils.FindModuleType<ModuleTypeBioDome>().GetComponentTypes();

            bioDomeComponents.Add(ComponentTypeList.find<StarchlessPeaPad>());
            bioDomeComponents.Add(ComponentTypeList.find<StarchlessRicePad>());
            bioDomeComponents.Add(ComponentTypeList.find<StarchlessPotatoPad>());
            bioDomeComponents.Add(ComponentTypeList.find<StarchlessWheatPad>());
            bioDomeComponents.Add(ComponentTypeList.find<StarchlessMaizePad>());

            BuildableUtils.FindModuleType<ModuleTypeBioDome>().SetComponentTypes(bioDomeComponents);
        }
    }
    public class StarchlessPeaPad : VegetablePad
    {
        public const string Name = "Starchless Pea Pad";
        
        public StarchlessPeaPad()
        {
            //this.mIcon = loadIcon();
            mIcon = loadIcon(TypeList<ResourceType, ResourceTypeList>.find<Vegetables>().getStatsColor());
            addUsageAnimation(CharacterAnimationType.WorkPadLow);
            addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad);
            addResourceProduction<Vegetables>(ResourceSubtype.Peas);
            addResourceProduction<Vegetables>(ResourceSubtype.Peas);
            this.mFlags = 2048;
            this.initStrings();
            this.mPrefabName = "PrefabPadStarchyPea";
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<PeaPad>().GetType().Name));
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starchless_pea_pad", Name);
            StringUtils.RegisterString("tooltip_starchless_pea_pad", StarchlessPlants.Description);
        }
    }
    public class StarchlessRicePad : VegetablePad
    {
        public const string Name = "Starchless Rice Pad";

        public StarchlessRicePad()
        {
            //this.mIcon = loadIcon();
            mIcon = loadIcon(TypeList<ResourceType, ResourceTypeList>.find<Vegetables>().getStatsColor());
            addUsageAnimation(CharacterAnimationType.WorkPadLow);
            addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad);
            addResourceProduction<Vegetables>(ResourceSubtype.Rice);
            addResourceProduction<Vegetables>(ResourceSubtype.Rice);
            this.mFlags = 2048;
            this.initStrings();
            this.mPrefabName = "PrefabPadStarchyRice";
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<RicePad>().GetType().Name));
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starchless_rice_pad", Name);
            StringUtils.RegisterString("tooltip_starchless_rice_pad", StarchlessPlants.Description);
        }
    }
    public class StarchlessPotatoPad : VegetablePad
    {
        public const string Name = "Starchless Potato Pad";

        public StarchlessPotatoPad()
        {
            //this.mIcon = loadIcon();
            mIcon = loadIcon(TypeList<ResourceType, ResourceTypeList>.find<Vegetables>().getStatsColor());
            addUsageAnimation(CharacterAnimationType.WorkPadLow);
            addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad);
            addResourceProduction<Vegetables>(ResourceSubtype.Potatoes);
            addResourceProduction<Vegetables>(ResourceSubtype.Potatoes);
            this.mFlags = 2048;
            this.initStrings();
            this.mPrefabName = "PrefabPadStarchyPotato";
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<PotatoPad>().GetType().Name));
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starchless_potato_pad", Name);
            StringUtils.RegisterString("tooltip_starchless_potato_pad", StarchlessPlants.Description);
        }
    }
    public class StarchlessWheatPad : VegetablePad
    {
        public const string Name = "Starchless Wheat Pad";

        public StarchlessWheatPad()
        {
            //this.mIcon = loadIcon();
            mIcon = loadIcon(TypeList<ResourceType, ResourceTypeList>.find<Vegetables>().getStatsColor());
            addUsageAnimation(CharacterAnimationType.WorkPadLow);
            addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad);
            addResourceProduction<Vegetables>(ResourceSubtype.Wheat);
            addResourceProduction<Vegetables>(ResourceSubtype.Wheat);
            this.mFlags = 2048;
            this.initStrings();
            this.mPrefabName = "PrefabPadStarchyWheat";
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<WheatPad>().GetType().Name));
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starchless_wheat_pad", Name);
            StringUtils.RegisterString("tooltip_starchless_wheat_pad", StarchlessPlants.Description);
        }
    }
    public class StarchlessMaizePad : VegetablePad
    {
        public const string Name = "Starchless Maize Pad";

        public StarchlessMaizePad()
        {
            //this.mIcon = loadIcon();
            mIcon = loadIcon(TypeList<ResourceType, ResourceTypeList>.find<Vegetables>().getStatsColor());
            addUsageAnimation(CharacterAnimationType.WorkPadLow);
            addUsageAnimation(CharacterAnimationType.TakeNotes, CharacterProp.Pad);
            addResourceProduction<Vegetables>(ResourceSubtype.Maize);
            addResourceProduction<Vegetables>(ResourceSubtype.Maize);
            this.mFlags = 2048;
            this.initStrings();
            this.mPrefabName = "PrefabPadStarchyMaize";
        }

        public new Texture2D loadIcon()
        {
            return ResourceUtil.loadIconColor("Components/icon_" + Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<MaizePad>().GetType().Name));
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("component_starchless_maize_pad", Name);
            StringUtils.RegisterString("tooltip_starchless_maize_pad", StarchlessPlants.Description);
        }
    }
}
