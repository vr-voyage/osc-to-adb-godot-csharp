using AdbGodotSharp;
using Godot;
using Godot.Collections;
using System;

public partial class UserDefinedOscClickerSetter : ColorRect
{
	private UserDefinedLocationClickerResource resource = new UserDefinedLocationClickerResource();

	[Export]
	public UserDefinedLocationClickerResource CurrentClickerSettings {
		get
		{
			return this.resource;
		}
		set
		{
			this.resource = value;
			ReloadResource();
		}
	}

	[Export]
	public ScaledTextureRect PhoneScreen { get; set; }

	bool Moving {get; set;} = false;
	Vector2 PhoneScreenPosition {get; set;} = Vector2.Zero;
	Viewport ViewPort {get; set;} = null;

	Vector2 Limits { get; set; } = Vector2.Zero;
	Vector2 HalfSize { get; set; } = Vector2.Zero;

	[Signal]
	public delegate void ClickTriggeredEventHandler(Vector2 position);

	bool ConditionMetOnLastCheck { get; set; } = false;

	void ReloadResource()
	{
		Color = CurrentClickerSettings.Color;
		Vector2 scaledPosition = CurrentClickerSettings.Position * PhoneScreen.CurrentScale;
		Limits = PhoneScreen.Size - Size;
		GD.Print($"[Clicker Setter] Position : {CurrentClickerSettings.Position} * {PhoneScreen.CurrentScale} = {scaledPosition}");
		Position = ClampPositionToParent(scaledPosition);
	}

	public Vector2 ClampPositionToParent(Vector2 pos)
	{
		GD.Print($"Clamping between {Vector2.Zero} and {Limits}");
		return pos.Clamp(Vector2.Zero, Limits);
	}


	public override void _Process(double delta)
	{
		if (Moving)
		{
			Vector2 newPosition = 
				(ViewPort.GetMousePosition() - PhoneScreenPosition - HalfSize);
			Position = ClampPositionToParent(newPosition);
		}
	}

	public override void _Ready()
	{
		HalfSize = Size / 2f;
	}

	public void OnGuiEvent(InputEvent @event)
	{
		GD.Print("Mouse event !");
		InputEventMouseButton mouseButtonEvent = @event as InputEventMouseButton;
		if (mouseButtonEvent != null)
		{
			Moving = mouseButtonEvent.Pressed;
			Vector2 phoneScreenPosition = Vector2.Zero;
			Vector2 limits = Vector2.Zero;
			if (PhoneScreen != null)
			{
				phoneScreenPosition = PhoneScreen.GlobalPosition;
				limits = PhoneScreen.Size - Size;
				limits.X = (limits.X >= 0f ? limits.X : 0f);
				limits.Y = (limits.Y >= 0f ? limits.Y : 0f);
			}
			PhoneScreenPosition = phoneScreenPosition;
			Limits = limits;
			
			ViewPort = GetViewport();
			if (!Moving)
			{
				CurrentClickerSettings.Position = Position * PhoneScreen.ReverseScale;
			}
		}
	}

	public void SelectedClickerChanged(UserDefinedLocationClickerResource resource)
	{
		if (resource == null)
		{
			GD.Print("[UserDefinedOscClickerSetter] SelectedClickerChanged - Display null !");
			Visible = false;
			CurrentClickerSettings = new UserDefinedLocationClickerResource();
		}
		else
		{
			GD.Print("[UserDefinedOscClickerSetter] SelectedClickerChanged - Display Not Null !");
			Visible = true;
			CurrentClickerSettings = resource;
		}
	}

	public void OscValuesChanged(OscValuesRead values)
	{
		if (!CurrentClickerSettings.Enabled) return;
		GD.Print("Osc Values Changed");
		OscActionConditionResource condition = CurrentClickerSettings.Condition;

		bool conditionMet = values.IsConditionMet(condition);
		if (!ConditionMetOnLastCheck && conditionMet)
		{
			ConditionMetOnLastCheck = true;
			EmitSignal(SignalName.ClickTriggered, CurrentClickerSettings.Position);
		}
		else if (!conditionMet)
		{
			ConditionMetOnLastCheck = false;
		}
	}

}
