using Godot;
using System;
using System.Collections.Generic;
using AdbGodotSharp;

public partial class DisplayOscMap : Control
{
	[Export]
	public PackedScene valueDisplayer;
	[Export]
	public OscServer server;

	[Signal]
	public delegate void ItemDoubleClickedEventHandler(string valueName, Variant value);

	public void ClearChildren()
	{
		int nChildren = GetChildCount();
		
		for (int i = nChildren - 1; i >= 0; i--)
		{
			RemoveChild(GetChild(i));
		}
	}

	public override void _EnterTree()
	{
		server.OscValuesUpdated += DisplayMap;
	}

	public override void _ExitTree()
	{
		server.OscValuesUpdated -= DisplayMap;
	}

	void ChildDoubleClicked(string valueName, Variant value)
	{
		EmitSignal(SignalName.ItemDoubleClicked, valueName, value);
	}

	public void DisplayMap(OscValuesRead oscValuesRead)
	{
		ClearChildren();

		foreach ((string valueName, Variant value) in oscValuesRead.Values)
		{
			DisplayOscValue display = valueDisplayer.Instantiate() as DisplayOscValue;
			if (display == null)
			{
				return;
			}
			display.ItemDoubleClicked += ChildDoubleClicked;
			AddChild(display);
			display.DisplayValue(valueName, value);
		}
	}
}

