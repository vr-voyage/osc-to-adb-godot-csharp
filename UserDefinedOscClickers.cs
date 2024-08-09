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
	public UserDefinedOscClickerContextMenu ClickerContextMenu { get; set; }

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
			display.ContextMenuRequested += HandleContextMenuRequests;
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
		OscClickers.LocationClickers.Add((UserDefinedLocationClickerResource)locationClicker);

		RefreshList();
		SelectLast();
	}
	
	public void ValueEdited(Resource locationClicker)
	{
		RefreshList();
	}

	public void AddOrEditValue(string path, Variant value)
	{
		var children = GetChildren();
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
		GD.Print($"Handle Delete Request of {clicker.Condition.Path}");
		if (selectedDisplay != null && selectedDisplay.ShownClicker == clicker)
		{
			selectedDisplay.Deselect();
			selectedDisplay = null;
			NotifySelectionChanged();
		}
		var deleted = OscClickers.LocationClickers.Remove(clicker);
		GD.Print($"Deleted ? {deleted}");
		RefreshList();
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

