using AdbGodotSharp;
using Godot;
using Godot.Collections;
using System;

public partial class UserDefinedOscClickers : Control
{
	[Export]
	public Control ListLocation { get; set; }

	[Export]
	public PackedScene OscClickerDisplay { get; set; }

	[Export]
	public UserDefinedOscClickerSetter ClickLocationSetter { get; set; }

	[Export]
	public UserDefinedOscClickerContextMenu ClickerContextMenu { get; set; }

	[Export]
	public string ClickersSaveDataPath { get; set; } = "user://clickers.json";

	[Export]
	public UserDefinedOscClickersResource OscClickers { get; set; } = new UserDefinedOscClickersResource();


	public UserDefinedLocationClickerDisplay selectedDisplay;


	[Signal]
	public delegate void SelectionChangedEventHandler(UserDefinedLocationClickerResource selectedResource);

	UserDefinedOscClickersResource TryLoadSavedData()
	{
		string saveData = FileAccess.GetFileAsString(ClickersSaveDataPath);
		if (saveData == "") return null;

		return UserDefinedOscClickersResource.FromJson(saveData);
	}

	public void SaveClickers()
	{
		using FileAccess saveFile = FileAccess.Open(ClickersSaveDataPath, FileAccess.ModeFlags.Write);
		if (saveFile == null)
		{
			return;
		}
		saveFile.StoreString(OscClickers.ToJson());
		saveFile.Close();
	}

	public override void _Ready()
	{
		var savedClickers = TryLoadSavedData();
		if (savedClickers != null)
		{
			OscClickers = savedClickers;
			RefreshList();
		}
	}

	public override void _ExitTree()
	{
		SaveClickers();
	}

	void NotifySelectionChanged()
	{
		GD.Print("NotifySelectionChanged");
		EmitSignal(SignalName.SelectionChanged, selectedDisplay?.ShownClicker);
	}

	public void DeselectCurrent()
	{
		GD.Print("DeselectCurrent");
		selectedDisplay?.Deselected();
	}

	public void ChildDisplaySelected(UserDefinedLocationClickerDisplay newSelectedDisplay)
	{
		GD.Print("ChildDisplaySelected");
		if (selectedDisplay == newSelectedDisplay) return;

		selectedDisplay?.Deselect();
		selectedDisplay = newSelectedDisplay;

		NotifySelectionChanged();
	}

	public void ChildDisplayDeselected(UserDefinedLocationClickerDisplay deselectedDisplay)
	{
		GD.Print("ChildDisplayDeselected");
		if (selectedDisplay == deselectedDisplay)
		{
			selectedDisplay = null;
		}

		NotifySelectionChanged();

	}

	void RemoveChildren()
	{
		GD.Print("RemoveChildren");
		var children = ListLocation.GetChildren();
		int nChildren = children.Count;

		for (int i = nChildren - 1; i >= 0; i--)
		{
			ListLocation.RemoveChild(children[i]);
		}
	}

	public void RefreshList()
	{
		var previouslySelectedClicker = selectedDisplay?.ShownClicker;
		GD.Print("RefreshList");
		RemoveChildren();
		selectedDisplay = null;
		GD.Print("Add Elements");
		foreach (var locationClicker in OscClickers.LocationClickers)
		{
			var display = OscClickerDisplay.Instantiate<UserDefinedLocationClickerDisplay>();
			display.ShownClicker = locationClicker;
			display.DisplayDeselected += ChildDisplayDeselected;
			display.DisplaySelected += ChildDisplaySelected;
			display.ContextMenuRequested += HandleContextMenuRequests;
			ListLocation.AddChild(display);
			if (previouslySelectedClicker == locationClicker)
			{
				display.Selected();
			}
		}
		
	}

	public void SelectLast()
	{
		if (ListLocation.GetChildCount() == 0) return;
        ListLocation.GetChild<UserDefinedLocationClickerDisplay>(-1).Selected();
	}

	public void Add(Resource locationClicker)
	{
		var clicker = (UserDefinedLocationClickerResource)locationClicker;

		OscClickers.LocationClickers.Add(clicker);

		RefreshList();
		SelectLast();
	}


	public void ValueEdited(Resource locationClicker)
	{
		RefreshList();
		
	}

	public void AddOrEditValue(string path, Variant value)
	{
		var children = ListLocation.GetChildren();
		foreach (var child in children)
		{
			if (child is not UserDefinedLocationClickerDisplay)
			{
				continue;
			}
			UserDefinedLocationClickerDisplay display = (UserDefinedLocationClickerDisplay)child;
			UserDefinedLocationClickerResource oscClicker = display.ShownClicker;
			if (oscClicker == null) continue;

			OscActionConditionResource actionCondition = oscClicker.Condition;
			if (actionCondition == null) continue;

			if (actionCondition.Path == path)
			{
				display.Selected();
				return;
			}
		}

		UserDefinedLocationClickerResource newClicker = new()
		{
			Enabled = false,
			Condition = new()
			{
				Path = path,
				Threshold = value.ToFloat()
			}
		};
		Add(newClicker);
	}

	public void HandleContextMenuRequests(UserDefinedLocationClickerDisplay fromChildDisplay)
	{
		Rect2I popupLocation = new Rect2(
			GetViewport().GetMousePosition(),
			ClickerContextMenu.Size).ToInt();

		ClickerContextMenu.TargetClicker = fromChildDisplay.ShownClicker;
		ClickerContextMenu.Popup(popupLocation);
	}

	public void HandleDeleteRequest(UserDefinedLocationClickerResource clicker)
	{

		if (selectedDisplay != null && selectedDisplay.ShownClicker == clicker)
		{
			selectedDisplay.Deselect();
			selectedDisplay = null;
			NotifySelectionChanged();
		}
		var deleted = OscClickers.LocationClickers.Remove(clicker);

		RefreshList();
	}

	public void OnGuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton)
		{
			return;
		}
		InputEventMouseButton mouseButtonEvent = (InputEventMouseButton)@event;
		if (mouseButtonEvent.Pressed) return;

		if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			DeselectCurrent();
		}
	}

}

public static class VectorHelpers
{
	public static Vector2I ToInt(this Vector2 vector)
	{
		return new Vector2I((int)vector.X, (int)vector.Y);
	}

	public static Rect2I ToInt(this Rect2 rect)
	{
		return new Rect2I(rect.Position.ToInt(), rect.Size.ToInt());
	}
}
