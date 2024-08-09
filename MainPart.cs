using AdbGodotSharp;
using Godot;
using System;

public partial class MainPart : Control
{
	[Export]
	public AdbHandler AdbHandler { get; set; }
	
	[Export]
	public OscServer OscServer { get; set; }

	public void UseSettings(UserMainSettingsResource settings)
	{
		AdbHandler.SetSettings(settings);
		OscServer.SetSettings(settings);
	}

}
