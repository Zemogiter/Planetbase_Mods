using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace PowerSaver
{
    public class PowerSaver : ModBase
    {
        public static string PRIORITY_LIST_PATH = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\PowerSaver\\Settings\\PowerSaver.xml";
        public static string CONSOLE_ICON_PATH = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Planetbase\\Mods\\PowerSaver\\Textures\\GridManagementConsoleIcon.png";


        public static List<Type> DEFAULT_POWER_PRIORITY_LIST =
        [
            typeof(ModuleTypeBasePad),
            typeof(ModuleTypeSignpost),
            typeof(ModuleTypeStarport),
            typeof(ModuleTypeLandingPad),
            typeof(ModuleTypeRadioAntenna),
            typeof(ModuleTypeStorage),
            typeof(ModuleTypeRoboticsFacility),
            typeof(ModuleTypeMine),
            typeof(ModuleTypeFactory),
            typeof(ModuleTypeProcessingPlant),
            typeof(ModuleTypeLab),
            typeof(ModuleTypeWaterTank),
            typeof(ModuleTypeBar),
            typeof(ModuleTypeMultiDome),
            typeof(ModuleTypeAntiMeteorLaser),
            typeof(ModuleTypeTelescope),
            typeof(ModuleTypeControlCenter),
            typeof(ModuleTypeDorm),
            typeof(ModuleTypeCabin),
            typeof(ModuleTypeSickBay),
            typeof(ModuleTypeCanteen),
            typeof(ModuleTypeBioDome),
            typeof(ModuleTypeAirlock),
            typeof(ModuleTypeOxygenGenerator),
            typeof(ModuleTypeWaterExtractor)
        ];

        public static List<Type> DEFAULT_WATER_PRIORITY_LIST =
        [
            typeof(ModuleTypeLab),
            typeof(ModuleTypeBar),
            typeof(ModuleTypeMultiDome),
            typeof(ModuleTypeCanteen),
            typeof(ModuleTypeBioDome),
            typeof(ModuleTypeOxygenGenerator)
        ];

        public class SavingMode(int trigger)
        {
            public int trigger = trigger;
            public List<Type> typesToShutDown = [];
        }

        public static List<SavingMode> mPowerSavingModes;
        public static List<SavingMode> mWaterSavingModes;
        public static SavingMode mActivePowerSavingMode;
        public static SavingMode mActiveWaterSavingMode;

        public static List<Type> mPowerPriorityList;
        public static List<Type> mWaterPriorityList;

        public static new void Init(ModEntry modEntry) => InitializeMod(new PowerSaver(), modEntry, "PowerSaver");

        public override void OnInitialized(ModEntry modEntry)
        {
            if (!File.Exists(PRIORITY_LIST_PATH))
            {
                Debug.Log("[MOD] PowerManager couldn't find the settings file.");
                return;
            }

            if (!File.Exists(CONSOLE_ICON_PATH))
            {
                Debug.Log("[MOD] PowerManager couldn't find the new console's icon. Using one from the game.");
                return;
            }

            mPowerSavingModes = [];
            mWaterSavingModes = [];
            mPowerPriorityList = [];
            mWaterPriorityList = [];

            try
            {
                System.Reflection.Assembly gameAssembly = System.Reflection.Assembly.GetCallingAssembly();
                using XmlReader reader = XmlReader.Create(PRIORITY_LIST_PATH);

                // Read Power saving modes
                reader.ReadToFollowing("PowerSavingModes");
                XmlReader powerSavingModes = reader.ReadSubtree();
                while (powerSavingModes.ReadToFollowing("SavingMode"))
                {
                    powerSavingModes.MoveToFirstAttribute();
                    powerSavingModes.ReadAttributeValue();
                    if (!powerSavingModes.HasValue)
                        continue;

                    int trigger = Int32.Parse(powerSavingModes.Value);
                    if (powerSavingModes.ReadToFollowing("Module"))
                    {
                        SavingMode mode = new(trigger);
                        do
                        {
                            Type type = gameAssembly.GetType("Planetbase.ModuleType" + powerSavingModes.ReadElementContentAsString(), false, true);
                            if (type != null)
                                mode.typesToShutDown.Add(type);
                        } while (powerSavingModes.ReadToNextSibling("Module"));

                        mPowerSavingModes.Add(mode);
                    }
                }

                // Read water saving modes
                reader.ReadToFollowing("WaterSavingModes");
                XmlReader waterSavingModes = reader.ReadSubtree();
                while (waterSavingModes.ReadToFollowing("SavingMode"))
                {
                    waterSavingModes.MoveToFirstAttribute();
                    waterSavingModes.ReadAttributeValue();
                    if (!waterSavingModes.HasValue)
                        continue;

                    int trigger = Int32.Parse(waterSavingModes.Value);
                    if (waterSavingModes.ReadToFollowing("Module"))
                    {
                        SavingMode mode = new(trigger);
                        do
                        {
                            Type type = gameAssembly.GetType("Planetbase.ModuleType" + waterSavingModes.ReadElementContentAsString(), false, true);
                            if (type != null)
                                mode.typesToShutDown.Add(type);
                        } while (waterSavingModes.ReadToFollowing("Module"));

                        mWaterSavingModes.Add(mode);
                    }
                }

                // Read power priority list
                reader.ReadToFollowing("PowerList");
                XmlReader powerList = reader.ReadSubtree();
                while (powerList.ReadToFollowing("Module"))
                {
                    Type type = gameAssembly.GetType("Planetbase.ModuleType" + powerList.ReadElementContentAsString(), false, true);
                    if (type != null)
                        mPowerPriorityList.Add(type);
                }

                // Read water priority list
                reader.ReadToFollowing("WaterList");
                XmlReader waterList = reader.ReadSubtree();
                while (waterList.ReadToFollowing("Module"))
                {
                    Type type = gameAssembly.GetType("Planetbase.ModuleType" + waterList.ReadElementContentAsString(), false, true);
                    if (type != null)
                        mWaterPriorityList.Add(type);
                }
            }
            catch (Exception e)
            {
                Debug.Log("<MOD> PowerManager failed to load the settings file. Exception: " + e.Message);
                return;
            }
            mPowerSavingModes = [.. mPowerSavingModes.OrderBy(m => m.trigger)];
            mWaterSavingModes = [.. mWaterSavingModes.OrderBy(m => m.trigger)];
            mActivePowerSavingMode = null;
            mActiveWaterSavingMode = null;

            foreach (Type type in DEFAULT_POWER_PRIORITY_LIST)
            {
                if (!mPowerPriorityList.Contains(type))
                    mPowerPriorityList.Insert(0, type);
            }

            foreach (Type type in DEFAULT_WATER_PRIORITY_LIST)
            {
                if (!mWaterPriorityList.Contains(type))
                    mWaterPriorityList.Insert(0, type);
            }

            TypeList<ComponentType, ComponentTypeList>.getInstance().AddType(new GridManagementConsole());
            ModuleTypeControlCenter controlCenter = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeControlCenter>() as ModuleTypeControlCenter;
            List<ComponentType> components = [.. controlCenter.GetComponentTypes()];
            components.Insert(3, TypeList<ComponentType, ComponentTypeList>.find<GridManagementConsole>());
            //controlCenter.mComponentTypes = [.. components];
            controlCenter.SetComponentTypes(components);

            Debug.Log("[MOD] PowerSaver activated");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            bool consoleExists = false;
            var components = CoreUtils.GetMember<List<ConstructionComponent>>("mComponents",);

            foreach (ConstructionComponent component in ConstructionComponent.mComponents)
            {
                bool lowCondition = component.getIndicators().isValidValue() && component.getIndicators().isExtremelyLow();
                if (component.getComponentType().GetType() == typeof(GridManagementConsole) && component.isBuilt() && !lowCondition && component.isEnabled() &&
                    component.mParentConstruction.isBuilt() && component.mParentConstruction.isEnabled() && !component.mParentConstruction.isExtremelyDamaged())
                {
                    consoleExists = true;
                    break;
                }
            }

            if (!consoleExists)
                return;

            Grid grid = Grid.getLargest();
            if (grid == null)
                return;

            if (mPowerSavingModes.Count > 0 && Module.getOverallPowerStorageCapacity() > 0f && (GetTotalConsumption(grid, GridResource.Power) > grid.getTotalPowerGeneration() || mActivePowerSavingMode != null))
            {
                float powerPercentage = (float)Module.getOverallPowerStorage() / Module.getOverallPowerStorageCapacity() * 100f;

                SavingMode newSavingMode = mPowerSavingModes.FirstOrDefault(m => powerPercentage <= m.trigger);
                if (newSavingMode != mActivePowerSavingMode)
                {
                    bool dontSwitch = false;
                    if (mActivePowerSavingMode != null && (newSavingMode == null || newSavingMode.trigger > mActivePowerSavingMode.trigger))
                        dontSwitch = powerPercentage < Mathf.Min(mActivePowerSavingMode.trigger * 1.2f, 100f);

                    if (!dontSwitch)
                        SwitchSavingMode(newSavingMode, GridResource.Power);
                }
            }

            if (mWaterSavingModes.Count > 0 && Module.getOverallWaterStorageCapacity() > 0f && (GetTotalConsumption(grid, GridResource.Water) > grid.getData(GridResource.Water).getGeneration() || mActiveWaterSavingMode != null))
            {
                float waterPercentage = (float)Module.getOverallWaterStorage() / Module.getOverallWaterStorageCapacity() * 100;

                SavingMode newSavingMode = mWaterSavingModes.FirstOrDefault(m => waterPercentage <= m.trigger);
                if (newSavingMode != mActiveWaterSavingMode)
                {
                    bool dontSwitch = false;
                    if (mActiveWaterSavingMode != null && (newSavingMode == null || newSavingMode.trigger > mActiveWaterSavingMode.trigger))
                        dontSwitch = waterPercentage < Mathf.Min(mActiveWaterSavingMode.trigger * 1.2f, 100f);

                    if (!dontSwitch)
                        SwitchSavingMode(newSavingMode, GridResource.Water);
                }
            }
        }

        public void SwitchSavingMode(SavingMode newSavingMode, GridResource resource)
        {
            Grid grid = Grid.getLargest();

            SavingMode currentSavingMode = resource == GridResource.Power ? mActivePowerSavingMode : mActiveWaterSavingMode;
            if (currentSavingMode != null)
            {
                // enable all types in this mode
                foreach (Type type in currentSavingMode.typesToShutDown)
                {
                    List<Construction> constructions = grid.mConstructions.Where(c => c is Module && (c as Module).mModuleType.GetType() == type).ToList();
                    foreach (Construction construction in constructions)
                    {
                        construction.setEnabled(true);
                    }
                }
            }

            if (newSavingMode != null)
            {
                // disable all types in the new mode
                bool skippedConsoleModule = false;
                foreach (Type type in newSavingMode.typesToShutDown)
                {
                    List<Construction> constructions = grid.mConstructions.Where(c => c is Module && (c as Module).mModuleType.GetType() == type).ToList();
                    foreach (Construction construction in constructions)
                    {
                        //if (module.mComponents.FirstOrDefault(c => c.mComponentType is GridManagementConsole) != null)
                        if (!skippedConsoleModule && type == typeof(ModuleTypeControlCenter) && construction.mComponents.FirstOrDefault(c => c.mComponentType is GridManagementConsole) != null)
                        {
                            skippedConsoleModule = true;
                            continue;
                        }
                        construction.setEnabled(false);
                    }
                }
            }

            if (resource == GridResource.Power)
                mActivePowerSavingMode = newSavingMode;
            else
                mActiveWaterSavingMode = newSavingMode;
        }

        public float GetTotalConsumption(Grid grid, GridResource resource)
        {
            float total = 0;
            foreach (Construction construction in grid.mConstructions)
            {
                if (construction.isBuilt() && !construction.isExtremelyDamaged())
                {
                    float generation = grid.getGeneration(resource);
                    if (generation < 0f)
                        total -= generation;
                }
            }

            return total;
        }
    }
}
