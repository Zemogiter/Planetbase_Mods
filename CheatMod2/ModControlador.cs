using Planetbase;
using UnityEngine;

namespace CheatModX;

public class ModControlador : MonoBehaviour
{
    public ModControlador()
    {
        Validar();
    }

    public void Validar()
    {
        TypeList<ComponentType, ComponentTypeList> instance = TypeList<ComponentType, ComponentTypeList>.getInstance();
        if (!instance.mTypeDictionary.ContainsKey("ModSpawnColono"))
        {
            instance.add(new ModSpawnColono());
        }
        if (!instance.mTypeDictionary.ContainsKey("ModContenedor"))
        {
            instance.add(new ModContenedor());
        }
        if (!instance.mTypeDictionary.ContainsKey("SuiteBed"))
        {
            instance.add(new SuiteBed());
        }
        TypeList<ModuleType, ModuleTypeList> instance2 = TypeList<ModuleType, ModuleTypeList>.getInstance();
        if (!instance2.mTypeDictionary.ContainsKey("ModuleTypeHyperdome"))
        {
            instance2.add(new ModuleTypeHyperdome());
        }
        if (!instance2.mTypeDictionary.ContainsKey("SuperDorm"))
        {
            instance2.add(new SuperDorm());
        }
        if (!instance2.mTypeDictionary.ContainsKey("Hospital"))
        {
            instance2.add(new Hospital());
        }
        if (!instance2.mTypeDictionary.ContainsKey("ShipMonument"))
        {
            instance2.add(new ShipMonument());
        }
        if (!instance2.mTypeDictionary.ContainsKey("Suite"))
        {
            instance2.add(new Suite());
        }
        TypeList<Specialization, SpecializationList> instance3 = TypeList<Specialization, SpecializationList>.getInstance();
        if (!instance3.mTypeDictionary.ContainsKey("Humanoid"))
        {
            instance3.add(new Humanoid());
        }
        if (!instance3.mTypeDictionary.ContainsKey("Drone"))
        {
            instance3.add(new Drone());
        }
    }

    public void Eliminar()
    {
        if (TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeDictionary.ContainsKey("ModSpawnColono"))
        {
            ModuleType moduleType = TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeHyperdome>();
            if (moduleType != null)
            {
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeList.Remove(moduleType);
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeDictionary.Remove("ModuleTypeHyperdome");
            }
            ModuleType moduleType2 = TypeList<ModuleType, ModuleTypeList>.find<SuperDorm>();
            if (moduleType2 != null)
            {
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeList.Remove(moduleType2);
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeDictionary.Remove("SuperDorm");
            }
            ModuleType moduleType3 = TypeList<ModuleType, ModuleTypeList>.find<Hospital>();
            if (moduleType3 != null)
            {
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeList.Remove(moduleType3);
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeDictionary.Remove("Hospital");
            }
            ModuleType moduleType4 = TypeList<ModuleType, ModuleTypeList>.find<ShipMonument>();
            if (moduleType4 != null)
            {
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeList.Remove(moduleType4);
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeDictionary.Remove("ShipMonument");
            }
            ModuleType moduleType5 = TypeList<ModuleType, ModuleTypeList>.find<Suite>();
            if (moduleType5 != null)
            {
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeList.Remove(moduleType5);
                TypeList<ModuleType, ModuleTypeList>.getInstance().mTypeDictionary.Remove("Suite");
            }
            ComponentType item = TypeList<ComponentType, ComponentTypeList>.find<ModSpawnColono>();
            if (moduleType != null)
            {
                TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeList.Remove(item);
                TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeDictionary.Remove("ModSpawnColono");
            }
            item = TypeList<ComponentType, ComponentTypeList>.find<ModContenedor>();
            if (moduleType != null)
            {
                TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeList.Remove(item);
                TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeDictionary.Remove("ModContenedor");
            }
            ComponentType componentType = TypeList<ComponentType, ComponentTypeList>.find<SuiteBed>();
            if (componentType != null)
            {
                TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeList.Remove(componentType);
                TypeList<ComponentType, ComponentTypeList>.getInstance().mTypeDictionary.Remove("SuiteBed");
            }   
            Specialization specialization = TypeList<Specialization, SpecializationList>.find<Humanoid>();
            if (specialization != null)
            {
                TypeList<Specialization, SpecializationList>.getInstance().mTypeList.Remove(specialization);
                TypeList<Specialization, SpecializationList>.getInstance().mTypeDictionary.Remove("Humanoid");
            }
            Specialization specialization2 = TypeList<Specialization, SpecializationList>.find<Drone>();
            if (specialization2 != null)
            {
                TypeList<Specialization, SpecializationList>.getInstance().mTypeList.Remove(specialization2);
                TypeList<Specialization, SpecializationList>.getInstance().mTypeDictionary.Remove("Drone");
            }
        }
    }

    public void OnGUI()
    {
        Singleton<GlobalVars>.getInstance().OnGUI();
    }

    public void init()
    {
        Resources.UnloadUnusedAssets();
    }
}
