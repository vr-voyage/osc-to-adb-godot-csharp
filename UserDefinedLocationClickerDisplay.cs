using AdbGodotSharp;
using Godot;
using System;

public partial class UserDefinedLocationClickerDisplay : PanelContainer
{


	[Export]
	public ColorRect colorRect;

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

	public void Display()
	{
		if (!IsNodeReady())
		{
			return;
		}

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
		if (@event is InputEventMouseButton)
		{
			var mouseClick = @event as InputEventMouseButton;
			if (mouseClick.ButtonIndex == MouseButton.Left && !mouseClick.Pressed)
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

	private void OnCheckBoxToggled(bool toggled)
	{
		if (shownClicker != null)
		{
			shownClicker.Enabled = toggled;
		}
	}
}


