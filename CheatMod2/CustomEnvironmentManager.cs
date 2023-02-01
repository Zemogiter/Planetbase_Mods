using System.Xml;
using Planetbase;

namespace CheatModX;

public abstract class CustomEnvironmentManager : EnvironmentManager
{
	public CustomEnvironmentManager()
	{
	}

	public void serialize(XmlNode rootNode, string name)
	{
		XmlNode parent = Serialization.createNode(rootNode, name);
		Serialization.serializeFloat(parent, "time-of-day", base.mTimeIndicator.getValue());
		Serialization.serializeFloat(parent, "wind-indicator", base.mWindIndicator.getValue());
		Singleton<GlobalVars>.getInstance().ModSaveData(rootNode);
	}

	public void deserialize(XmlNode node)
	{
		base.mTimeIndicator.setValue(Serialization.deserializeFloat(node["time-of-day"]));
		base.mWindIndicator.setValue(Serialization.deserializeFloat(node["wind-indicator"]));
		base.mPreviousRenderParameters = null;
		base.updateDayNightCycle();
		Singleton<GlobalVars>.getInstance().ModLoadData(node);
	}
}
