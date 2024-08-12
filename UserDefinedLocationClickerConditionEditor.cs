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

	[Export]
	public CheckBox LongPressCheck { get; set; }

	[Export]
	public SpinBox PressTime { get; set; }

	// True if Editing an existing resource
	// False if Preparing to add an existing resource
	private bool editing = false;
	public bool Editing
	{
		get
		{
			return editing;
		}
		set
		{
			if (value == false)
			{
				ButtonAdd.Show();
				ButtonEdit.Hide();
				editing = false;
			}
			else
			{
				ButtonAdd.Hide();
				ButtonEdit.Show();
				editing = true;
			}
		}
	}

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
				currentResource = value;
				Editing = true;

			}
			else
			{
				currentResource = new UserDefinedLocationClickerResource();
				Editing = false;
			}
			RefreshUi();
		}
	}

	public void ResetUi()
	{
		OscPathEdit.Text = "";
		ConditionMenuSelect.Selected = 0;
		ThresholdEdit.Text = "";
	}

	public void OnThresholdSubmitted(String newText)
	{
		bool parsed = Single.TryParse(
			newText,
			System.Globalization.NumberStyles.Number,
			CultureInfo.InvariantCulture,
			out float parsedValue);

		if (!parsed)
		{
			ThresholdEdit.Text = CurrentResource.Condition.Threshold.ToString();
		}
		else
		{
			if (Editing)
			{
				ThresholdValidated(parsedValue);
			}
		}
	}

	public bool SaveValues()
	{
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
		ResetUi();
		OscActionConditionResource condition = CurrentResource.Condition;

		OscPathEdit.Text = condition.Path;
		ConditionMenuSelect.Selected = (int)condition.Condition;
		ThresholdEdit.Text = condition.Threshold.ToString(CultureInfo.InvariantCulture);
		bool isLongPress = CurrentResource.Type == UserDefinedLocationClickerResource.ClickerType.Long;
		PressTime.Editable = isLongPress;
		LongPressCheck.SetPressedNoSignal(isLongPress);
		PressTime.SetValueNoSignal(CurrentResource.PressTime);
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

	public void CurrentSelectionChanged(UserDefinedLocationClickerResource resource)
	{
		if (resource == null)
		{
			CurrentResource = null;
		}
		else
		{
			CurrentResource = resource;
		}
	}

	public override void _Ready()
	{
		ButtonAdd.Pressed += OnAddButtonClicked;
		ButtonEdit.Pressed += OnEditButtonClicked;
	}

	OscActionConditionResource CurrentCondition()
	{
		if (currentResource == null) return null;
		return currentResource.Condition;
	}

	void SubmitChangesIfEditing()
	{
		if (Editing)
		{
			EmitSignal(SignalName.ValuesEdited, CurrentResource);
		}
	}

	public void PathChangeSubmitted(string newPath)
	{
		var condition = CurrentCondition();
		if (condition == null) return;

		condition.Path = newPath;
		SubmitChangesIfEditing();

	}

	public void ConditionChanged(int newCondition)
	{
		var condition = CurrentCondition();
		if (condition == null) return;

		condition.Condition = (OscActionConditionEnum)ConditionMenuSelect.Selected;
		SubmitChangesIfEditing();
	}

	public void ThresholdValidated(float threshold)
	{
		var condition = CurrentCondition();
		if (condition == null) return;

		condition.Threshold = threshold;
		SubmitChangesIfEditing();
	}

	public void LongTimeCheckChanged(bool check)
	{
		PressTime.Editable = check;
		SaveTimings();
	}

	public void LongTimeDefined(float value)
	{
		SaveTimings();
	}

	void SaveTimings()
	{
		currentResource.Type = (
			LongPressCheck.ButtonPressed
			? UserDefinedLocationClickerResource.ClickerType.Long
			: UserDefinedLocationClickerResource.ClickerType.Instant);
		CurrentResource.PressTime = (float)PressTime.Value;
		SubmitChangesIfEditing();
	}
}

