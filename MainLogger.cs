using Godot;
using System;
using System.Collections.Generic;

public partial class MainLogger : Control
{
	public List<String> messages = new List<String>(64);

	[Signal]
	public delegate void LogMessageAddedEventHandler(string message);

	public void LogMessage(string message)
	{
		messages.Add(message);
		EmitSignal(SignalName.LogMessageAdded, message);
	}
}
