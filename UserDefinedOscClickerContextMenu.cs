using AdbGodotSharp;
using Godot;
using System;

public partial class UserDefinedOscClickerContextMenu : PopupMenu
{
	public UserDefinedLocationClickerResource TargetClicker { get; set; }

	[Signal]
	public delegate void InvokedDeleteEventHandler(UserDefinedLocationClickerResource clicker);

	private const int deleteItemLocation = 2;

	public void OnMenuItemPressed(long index)
	{
		if (TargetClicker == null)
		{
			return;
		}

		if (index == deleteItemLocation)
		{
			GD.Print("Invoked deletion !");
			EmitSignal(SignalName.InvokedDelete, TargetClicker);
			TargetClicker = null;
		}
	}
}



