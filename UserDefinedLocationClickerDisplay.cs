using AdbGodotSharp;
using Godot;
using System;

public partial class UserDefinedLocationClickerDisplay : PanelContainer
{

	[Export]
	public CheckBox enabledCheck;

	[Export]
	public ColorPickerButton colorRect;

	[Export]
	public Label oscPathText;

	[Export]
	public Label conditionText;

	[Export]
	public Label thresholdText;

	[Export]
	public StyleBox SelectedStyle { get; set; }

	[Export]
	public StyleBox NotSelectedStyle { get; set; }

	[Signal]
	public delegate void ClickerResourceUpdatedEventHandler(UserDefinedLocationClickerResource resource);

	bool selected = false;

	private UserDefinedLocationClickerResource shownClicker;
	public UserDefinedLocationClickerResource ShownClicker
	{
		get
		{
			return shownClicker;
		}
		set
		{
			shownClicker = value;
			Display();
		}
	}

	[Signal]
	public delegate void DisplaySelectedEventHandler(UserDefinedLocationClickerDisplay display);

	[Signal]
	public delegate void DisplayDeselectedEventHandler(UserDefinedLocationClickerDisplay display);

	[Signal]
	public delegate void ContextMenuRequestedEventHandler(UserDefinedLocationClickerDisplay display);


	public void Display()
	{
		if (!IsNodeReady())
		{
			return;
		}

		enabledCheck.SetPressedNoSignal(ShownClicker.Enabled);
		colorRect.Color = ShownClicker.Color;
		OscActionConditionResource actionCondition = ShownClicker.Condition;
		oscPathText.Text = actionCondition.Path;
		conditionText.Text = actionCondition.Condition.ToReadableString();
		thresholdText.Text = actionCondition.Threshold.ToString();
	}

	public override void _Ready()
	{		
		Display();
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton)
		{
			return;
		}

		var mouseClick = @event as InputEventMouseButton;
		if (mouseClick.Pressed) return;
			
		if (mouseClick.ButtonIndex == MouseButton.Left)
		{
			GD.Print("[UserDefinedLocationClickerDisplay] Clicked");
			if (!selected)
			{
				Selected();
			}
			else
			{
				Deselected();
			}
		}
		if (mouseClick.ButtonIndex == MouseButton.Right)
		{
			RequestingContextualMenu();
		}
	}

	public void Selected()
	{
		GD.Print("[UserDefinedLocationClickerDisplay] Selected");
		selected = true;
		EmitSignal(SignalName.DisplaySelected, this);
		RemoveThemeStyleboxOverride("panel");
		AddThemeStyleboxOverride("panel", SelectedStyle);
	}

	public void Deselect()
	{
		GD.Print("[UserDefinedLocationClickerDisplay] Deselect");
		selected = false;
		RemoveThemeStyleboxOverride("panel");
		AddThemeStyleboxOverride("panel", NotSelectedStyle);
	}

	public void Deselected()
	{
		GD.Print("[UserDefinedLocationClickerDisplay] Deselected");
		Deselect();
		EmitSignal(SignalName.DisplayDeselected, this);
	}

	public void RequestingContextualMenu()
	{
		EmitSignal(SignalName.ContextMenuRequested, this);
	}

	private void OnCheckBoxToggled(bool toggled)
	{
		if (shownClicker != null)
		{
			shownClicker.Enabled = toggled;
		}
		EmitSignal(SignalName.ClickerResourceUpdated, shownClicker);
	}

	private void OnColorSelected(Color newColor)
	{
		if (shownClicker != null)
		{
			shownClicker.Color = newColor;
		}
		EmitSignal(SignalName.ClickerResourceUpdated, shownClicker);
	}
}

