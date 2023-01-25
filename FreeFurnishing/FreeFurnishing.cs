using Planetbase;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using UnityEngine;
using System.Reflection;
using Module = Planetbase.Module;

namespace FreeFurnishing
{
    public class FreeFurnishing : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new FreeFurnishing(), modEntry, "FreeFurnishing");

        public override void OnInitialized(ModEntry modEntry)
        {
            Debug.Log("[MOD] FreeFurnishing activated");

        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            GameObject go = new GameObject();
            go.AddComponent<ComponentX>();
            GameObject.DontDestroyOnLoad(go);
        }
    }
    public class ComponentX : MonoBehaviour
    {
        public void OnGUI()
        {
            typeof(DebugManager).GetField("mEnabled", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(DebugManager.getInstance(), true);
            DebugManager.getInstance().onGui();
        }
    }

    public class CustomModule : Module
    {
        public bool CanPlaceComponent(ConstructionComponent component)
        {
            Module module = new Module();
            if (Input.GetKeyUp(KeyCode.X))
            {
                // rotate
                component.getTransform().Rotate(Vector3.up * 15f);
            }

            // step
            Vector3 fromCenter = component.getPosition() - getPosition();
            fromCenter.x = Mathf.Round(fromCenter.x * 2f) * 0.5f;
            fromCenter.z = Mathf.Round(fromCenter.z * 2f) * 0.5f;
            component.setPosition(getPosition() + fromCenter);

            CoreUtils.InvokeMethod<Module>("clampComponentPosition", module, component);
            clampComponentPosition(component);

            return !CoreUtils.InvokeMethod<Module, bool>("intersectsAnyComponents", module, component);
        }
    }
}
