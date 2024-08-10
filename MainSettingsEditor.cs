using AdbGodotSharp;
using Godot;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public partial class MainSettingsEditor : PanelContainer
{
	class SearchResult
	{
		public string[] foundPaths;
	}

	[Signal]
	public delegate void SettingsSavedEventHandler();

	[Export]
	public UserMainSettingsResource MainSettings { get; set; }

	[Export]
	public LineEdit AdbPathEdit { get; set; }

	[Export]
	public FileDialog AdbPathSelector { get; set; }

	[Export]
	public ItemList FoundAdbPaths { get; set; }

	[Export]
	public Button SearchButton { get; set; }

	public string SearchDirPath { get; set; } = "C:\\Program Files\\Unity";
	public string SearchBinaryName { get; set; } = "adb.exe";

	Thread SearchThread { get; set; }
	SearchResult SearchResults { get; set; }

	/* FIXME : Find a way to avoid the duplication */
	[Export]
	public string SettingsFilePath { get; set; } = "user://MainSettings.json";

	public void Save()
	{
		MainSettings ??= new UserMainSettingsResource();

		MainSettings.adbPath = AdbPathEdit.Text;
		using Godot.FileAccess saveFile = Godot.FileAccess.Open(SettingsFilePath, Godot.FileAccess.ModeFlags.Write);
		saveFile.StoreString(MainSettings.ToJson());
		saveFile.Close();
		EmitSignal(SignalName.SettingsSaved);
	}

	public override void _Ready()
	{
		if (MainSettings != null)
		{
			AdbPathEdit.Text = MainSettings.adbPath;
		}
	}

	public void OnAdbExeFoundListItemSelected(int item)
	{
		OnAdbExeFilePathSelected(FoundAdbPaths.GetItemText(item));
	}

	public void OnAdbExeFilePathSelected(string filePath)
	{
		AdbPathEdit.Text = filePath;
	}

	private void SelectAdbFileButtonPressed()
	{
		AdbPathSelector.PopupCenteredRatio(0.9f);
	}

	void SearchButtonSearchingMode()
	{
		SearchButton.Disabled = true;
		SearchButton.Text = "Searching...";
	}

	void SearchButtonReadyMode()
	{
		SearchButton.Disabled = false;
		SearchButton.Text = $"Search for {SearchBinaryName}";
	}

	public void StartSearchForAdbExe()
	{
		SearchButtonSearchingMode();

		if (SearchThread != null && SearchThread.ThreadState != ThreadState.Stopped)
		{
			return;
		}
		SearchThread = new Thread(SearchForAdbExe);
		SearchResults = new SearchResult();

		SearchThread.Start(SearchResults);
		SetProcess(true);
	}

	public void SearchForAdbExe(object threadDataUncasted)
	{
		var searchResults = (SearchResult)threadDataUncasted;
		searchResults.foundPaths = Directory.GetFiles(SearchDirPath, SearchBinaryName, SearchOption.AllDirectories);
	}

	void DisplayLastSearchResults()
	{
		FoundAdbPaths.Clear();
		foreach (string foundPath in SearchResults.foundPaths)
		{
			FoundAdbPaths.AddItem(foundPath);
		}
	}

	void CleanupThreads()
	{
		SearchThread = null;
		SearchResults = null;
	}

	public override void _Process(double delta)
	{
		if (SearchThread == null)
		{
			SetProcess(false);
			return;
		}

		if (SearchThread.ThreadState != ThreadState.Stopped)
		{
			return;
		}

		DisplayLastSearchResults();
		CleanupThreads();
		SearchButtonReadyMode();

		SetProcess(false);
	}

	public void OnSelectFoundAdbPath(string adbPath)
	{
		AdbPathEdit.Text = adbPath;
	}

}


