using Planetbase;
using UnityEngine;

namespace CheatModX;

public class MyWindow : GuiWindow
{
	public float Size = (float)Screen.height * 0.8f;

	public MyWindow()
		: base(new GuiLabelItem("Cheat MOD", ResourceList.StaticIcons.Welfare))
	{
	}

	public override float getWidth()
	{
		return Size;
	}
}
