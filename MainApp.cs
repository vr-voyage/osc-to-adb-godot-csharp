using AdbGodotSharp;
using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class MainApp : Control
{
	[Export]
	public string SettingsFilePath { get; set; } = "user://MainSettings.res";

	[Export]
	public MainSettingsEditor SettingsEditor { get; set; }

	[Export]
	public MainPart MainPart { get; set; }

	bool AppInitialized { get; set; } = false;

	

	public UserMainSettingsResource LoadSettings()
	{
		return ResourceLoader.Load(SettingsFilePath).ToMainSettings();  
	}

	public bool CreateSaveFile()
	{
		var settings = new UserMainSettingsResource();
		return (ResourceSaver.Save(settings, SettingsFilePath, ResourceSaver.SaverFlags.OmitEditorProperties | ResourceSaver.SaverFlags.BundleResources) == Error.Ok);
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
		if (AppInitialized) return true;

		UserMainSettingsResource settings = LoadSettings();
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

public static class ResourceHelpers
{
	public static UserMainSettingsResource ToMainSettings(this Resource resource)
	{
		UserMainSettingsResource res = new UserMainSettingsResource();
		GD.Print($"{res.GetPropertyList()}");
		GD.Print($"{res.GetMetaList()}");

		res.adbPath = (string)resource.Get("adbPath");
		GD.Print($"Resource {resource.Get("adbPath")}");
		return res;
	}
}
