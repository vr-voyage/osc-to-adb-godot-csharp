using AdbGodotSharp;
using Godot;
using System;

public partial class MainSettingsEditor : PanelContainer
{
	[Signal]
	public delegate void SettingsSavedEventHandler();

	[Export]
	public UserMainSettingsResource MainSettings { get; set; }

	[Export]
	public LineEdit AdbPathEdit;

	[Export]
	public FileDialog AdbPathSelector;

	public string SettingsFilePath { get; set; } = "user://MainSettings.res";

	public void Save()
	{
		if (MainSettings == null)
		{
			MainSettings = new UserMainSettingsResource();
		}

		MainSettings.adbPath = AdbPathEdit.Text;
		ResourceSaver.Save(MainSettings, SettingsFilePath);
		EmitSignal(SignalName.SettingsSaved);
	}

	public override void _Ready()
	{
		if (MainSettings != null)
		{
			AdbPathEdit.Text = MainSettings.adbPath;
		}
	}

	public void OnAdbExeFilePathSelected(string filePath)
	{
		AdbPathEdit.Text = filePath;
	}

	private void SelectAdbFileButtonPressed()
	{
		AdbPathSelector.PopupCenteredRatio(0.9f);
	}

}



