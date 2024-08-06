using Godot;
using System;

public partial class DisplayOscValue : PanelContainer
{
	[Export]
	public Label uiValueName;
	
	[Export]
	public Label uiValue;

	public void DisplayValue(string valueName, Variant value)
	{
		uiValueName.Text = valueName;
		uiValue.Text = value.ToString();
	}
}
