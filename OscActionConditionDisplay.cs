using AdbGodotSharp;
using Godot;
using System;

public partial class OscActionConditionDisplay : HBoxContainer
{
	[Export]
	public ColorRect colorRect;

	[Export]
	public Label oscPathText;

	[Export]
	public Label conditionText;

	[Export]
	public Label thresholdText;

	public void Show(UserDefinedLocationClickerResource locationClicker)
	{
		colorRect.Color = locationClicker.Color;
		OscActionConditionResource actionCondition = locationClicker.Condition;
		oscPathText.Text = actionCondition.Path;
		conditionText.Text = actionCondition.Condition.ToReadableString();
		thresholdText.Text = actionCondition.Threshold.ToString();
	}
}
