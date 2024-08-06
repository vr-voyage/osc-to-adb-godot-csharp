using AdbGodotSharp;
using Godot;
using System;
using System.Globalization;

public partial class UserDefinedLocationClickerConditionEditor : VBoxContainer
{
	[Export]
	public LineEdit OscPathEdit { get; set; }

	[Export]
	public OptionButton ConditionMenuSelect { get; set; }

	[Export]
	public LineEdit ThresholdEdit { get; set; }

	[Export]
	public Button ButtonAdd { get; set; }

	[Export]
	public Button ButtonEdit { get; set; }

	[Signal]
	public delegate void AddOscClickerEventHandler(UserDefinedLocationClickerResource oscClicker);

	[Signal]
	public delegate void ValuesEditedEventHandler(UserDefinedLocationClickerResource oscClicker);

	private UserDefinedLocationClickerResource currentResource = new UserDefinedLocationClickerResource();
	public UserDefinedLocationClickerResource CurrentResource {
		get
		{
			return currentResource;
		}
		set
		{

			if (value != null)
			{
				GD.Print("Set non-null value");
				currentResource = value;
				ButtonAdd.Hide();
				ButtonEdit.Show();
			}
			else
			{
				GD.Print("Set null value");
				currentResource = new UserDefinedLocationClickerResource();
				ButtonAdd.Show();
				ButtonEdit.Hide();
			}
			RefreshUi();
		}
	}

	public void ResetUi()
	{
		GD.Print("Reset UI");
		OscPathEdit.Text = "";
		ConditionMenuSelect.Selected = 0;
		ThresholdEdit.Text = "";
	}

	public void OnTextSubmitted(String newText)
	{
		GD.Print("On Text Submitted");
		bool parsed = Single.TryParse(
			newText,
			System.Globalization.NumberStyles.Number,
			CultureInfo.InvariantCulture,
			out float _);

		if (!parsed)
		{
			ThresholdEdit.Text = CurrentResource.Condition.Threshold.ToString();
		}
	}

	public bool SaveValues()
	{
		GD.Print("Save Values");
		string thresholdText = ThresholdEdit.Text;
		bool parsed = Single.TryParse(
			thresholdText,
			System.Globalization.NumberStyles.Number,
			CultureInfo.InvariantCulture,
			out float threshold);

		if (!parsed)
		{
			return false;
		}
		OscActionConditionEnum condition = (OscActionConditionEnum)ConditionMenuSelect.Selected;
		string oscPath = OscPathEdit.Text;

		OscActionConditionResource setupCondition = CurrentResource.Condition;
		setupCondition.Path = oscPath;
		setupCondition.Condition = condition;
		setupCondition.Threshold = threshold;
		return true;
	}

	public void RefreshUi()
	{
		GD.Print("Refresh UI");
		ResetUi();
		OscActionConditionResource condition = CurrentResource.Condition;

		OscPathEdit.Text = condition.Path;
		ConditionMenuSelect.Selected = (int)condition.Condition;
		ThresholdEdit.Text = condition.Threshold.ToString();
		GD.Print($"{condition.Path} - {condition.Threshold}");
	}

	public void OnAddButtonClicked()
	{
		if (SaveValues())
		{
			EmitSignal(SignalName.AddOscClicker, CurrentResource);
		}

	}

	public void OnEditButtonClicked()
	{
		if (SaveValues())
		{
			EmitSignal(SignalName.ValuesEdited, CurrentResource);
		}
	}

	public void CurrentSelectionChanged(UserDefinedLocationClickerDisplay display)
	{
		GD.Print("CurrentSelectionChanged");
		if (display == null)
		{
			CurrentResource = null;
		}
		else
		{
			CurrentResource = display.ShownClicker;
		}
	}

	public override void _Ready()
	{
		ButtonAdd.Pressed += OnAddButtonClicked;
		ButtonEdit.Pressed += OnEditButtonClicked;
	}
}
