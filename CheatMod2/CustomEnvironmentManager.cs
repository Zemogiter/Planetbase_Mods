using System.Xml;
using Planetbase;
using HarmonyLib;

namespace CheatModX
{
    [HarmonyPatch(typeof(EnvironmentManager), nameof(EnvironmentManager.serialize))]
    public class SerializationPatch
    {
        static void Postfix(XmlNode rootNode)
        {
            Singleton<GlobalVars>.getInstance().ModSaveData(rootNode);
        }
    }
    [HarmonyPatch(typeof(EnvironmentManager), nameof(EnvironmentManager.deserialize))]
    public class DeserializationPatch
    {
        static void Postfix(XmlNode node)
        {
            Singleton<GlobalVars>.getInstance().ModLoadData(node);
        }
    }
}
