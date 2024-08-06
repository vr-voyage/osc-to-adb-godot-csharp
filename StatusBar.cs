using Godot;
using System;

public partial class StatusBar : Label
{
	public void ShowMessage(string message)
	{
		Text = message;
	}
}
