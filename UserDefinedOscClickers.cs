using AdbGodotSharp;
using Godot;
using System;

public partial class UserDefinedOscClickers : VBoxContainer
{
	[Export]
	public PackedScene OscClickerDisplay { get; set; }

	[Export]
	public UserDefinedOscClickerSetter ClickLocationSetter { get; set; }

	[Export]
	public UserDefinedOscClickersResource OscClickers { get; set; } = new UserDefinedOscClickersResource();


	public UserDefinedLocationClickerDisplay selectedDisplay;

	[Signal]
	public delegate void SelectionChangedEventHandler(UserDefinedLocationClickerResource selectedResource);

	void NotifySelectionChanged()
	{
		GD.Print("NotifySelectionChanged");
		EmitSignal(SignalName.SelectionChanged, selectedDisplay);
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
		var children = GetChildren();
		int nChildren = children.Count;

		for (int i = nChildren - 1; i >= 0; i--)
		{
			RemoveChild(children[i]);
		}
	}

	public void RefreshList()
	{
		GD.Print("RefreshList");
		RemoveChildren();
		GD.Print("Add Elements");
		foreach (var locationClicker in OscClickers.LocationClickers)
		{
			var display = OscClickerDisplay.Instantiate<UserDefinedLocationClickerDisplay>();
			display.ShownClicker = locationClicker;
			display.DisplayDeselected += ChildDisplayDeselected;
			display.DisplaySelected += ChildDisplaySelected;
			AddChild(display);
		}
	}

	public void SelectLast()
	{
		GD.Print("SelectLast");
		GetChild<UserDefinedLocationClickerDisplay>(-1).Selected();
	}

	public void Add(Resource locationClicker)
	{
		GD.Print("Add");
		GD.Print($"{locationClicker}");
		GD.Print($"{OscClickers}");
		GD.Print($"{OscClickers.LocationClickers}");
		OscClickers.LocationClickers.Add((UserDefinedLocationClickerResource)locationClicker);

		RefreshList();
		SelectLast();
	}
	
	public void ValueEdited(Resource locationClicker)
	{
		RefreshList();
	}

}

