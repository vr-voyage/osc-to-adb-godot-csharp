using Godot;
using System;

public partial class DisplayOscValue : PanelContainer
{
	[Export]
	public Label uiValueName;
	
	[Export]
	public Label uiValue;

	Variant displayedValue;

	[Signal]
	public delegate void OnItemDoubleClickedEventHandler(string valueName, Variant value);

	public event OnItemDoubleClickedEventHandler ItemDoubleClicked;

	public void DisplayValue(string valueName, Variant value)
	{
		uiValueName.Text = valueName;
		uiValue.Text = value.ToString();
		displayedValue = value;

	}

	public void OnGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			var mouseButtonEvent = @event as InputEventMouseButton;
			if (mouseButtonEvent.DoubleClick)
			{
				ItemDoubleClicked(uiValueName.Text, displayedValue);
			}
		}
	}
}



