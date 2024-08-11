using AdbGodotSharp;
using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class MainApp : Control
{
	[Export]
	public string SettingsFilePath { get; set; } = "user://MainSettings.json";

	[Export]
	public MainSettingsEditor SettingsEditor { get; set; }

	[Export]
	public MainPart MainPart { get; set; }

	bool AppInitialized { get; set; } = false;

	

	public UserMainSettingsResource LoadSettings()
	{

        return UserMainSettingsResource.FromJson(FileAccess.GetFileAsString(SettingsFilePath));
	}

	public bool CreateSaveFile()
	{
		var settings = new UserMainSettingsResource();
		using FileAccess saveFile = FileAccess.Open(SettingsFilePath, FileAccess.ModeFlags.Write);
		if (saveFile == null)
		{
			return false;
		}
		saveFile.StoreString(settings.ToJson());
		saveFile.Close();

		return true;
	}

	void ShowSettings()
	{
		MainPart.Visible = false;
		SettingsEditor.Visible = true;
	}

	void ShowMainApp()
	{
		SettingsEditor.Visible = false;
		MainPart.Visible = true;
		AppInitialized = true;
	}

	bool StartAppIfSettingsSeemCorrect()
	{
		if (AppInitialized)
		{
			return true;
		}

		GD.Print("StartAppBeforeLoad");
		UserMainSettingsResource settings = LoadSettings();
		GD.Print("StartAppAfterLoad");
		if (settings == null)
		{
			GD.Print("Could not load the settings from the disk");
			CreateSaveFile();
			settings = LoadSettings();
		}

		if (settings == null || !settings.AdbAppearUseable())
		{
			GD.Print("Could not load the settings !");
			if (settings == null)
			{
				GD.Print("Settings are still null");
			}
			else
			{
				GD.Print("ADB doesn't appear to be useable");
			}
			SettingsEditor.MainSettings = settings;
			return false;
		}

		MainPart.UseSettings(settings);
		ShowMainApp();
		return true;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!StartAppIfSettingsSeemCorrect())
		{
			ShowSettings();
		}

	}

	public void SettingsChanged()
	{
		if (!StartAppIfSettingsSeemCorrect())
		{
			ShowSettings();
		}
	}
}
