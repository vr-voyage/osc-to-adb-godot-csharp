using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public partial class MainUi : Control
{

	[Export(PropertyHint.GlobalFile, "*.exe")]
	public String adbExe;

	[Export]
	public ScaledTextureRect screenOutput;

	[Export]
	public Label statusOutput;

	[Export]
	public OscServer OscServer { get; set; }

	[Export]
	public UserDefinedOscClickerSetter clickerSetter { get; set; } 

	public class ThreadData
	{
		public String adbExeFilePath;
		public String arguments;
		public bool done = false;
		public byte[] output = new byte[0];
		public Process currentProcess = null;
	};
	ThreadData currentThreadData = null;

	public static byte[] CopyAdbOutput(ThreadData localData)
	{
		MemoryStream ms = new MemoryStream();
		localData.currentProcess.StandardOutput.BaseStream.CopyTo(ms);
		return ms.ToArray();
	}

	public static void TakeScreenshot(Object localDataUncasted)
	{
		ThreadData localData = (ThreadData)localDataUncasted;
		StartAdb(localData);
		localData.output = CopyAdbOutput(localData);
		localData.done = true;
	}

	public static void StartAdb(ThreadData localData)
	{
		localData.currentProcess = new Process 
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = localData.adbExeFilePath,
				Arguments = localData.arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = false,
				CreateNoWindow = true,
			}
		};
		localData.currentProcess.Start();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OscServer.OscValuesUpdated += clickerSetter.OscValuesChanged;

	}

	public override void _Process(double delta)
	{
		if (currentThreadData == null || currentThreadData.done == false)
		{
			return;
		}

		_ShowScreenshot(currentThreadData.output);
		currentThreadData = null;
	}

	public void _ShowScreenshot(byte[] screenshotData)
	{
		statusOutput.Text = (screenshotData.Length.ToString());
		var image = new Image();
		Godot.Error ret = image.LoadPngFromBuffer(screenshotData);
		if (((int) ret) != 0)
		{
			statusOutput.Text = "Could not load image !";
			return;
		}
		screenOutput.ApplyAndScaleTexture(ImageTexture.CreateFromImage(image));
	}

	private void _InvokeAdb(string adbPath, string arguments, ParameterizedThreadStart handler)
	{
		currentThreadData = new ThreadData();
		currentThreadData.adbExeFilePath = adbExe;
		currentThreadData.arguments = arguments;
		Thread newThread = new Thread(handler);
		newThread.Start(currentThreadData);
	}

	private void _TriggerScreenshot()
	{
		_InvokeAdb(adbExe, "exec-out screencap -p", MainUi.TakeScreenshot);
	}

	public void ClickTriggered(Vector2 position)
	{
		Vector2 actualPosition = position * screenOutput.ReverseScale;
		_ClickScreenAdb((int)actualPosition.X, (int)actualPosition.Y);
	}

	private void _ClickScreenAdb(int x, int y)
	{
		_InvokeAdb(adbExe, $"shell input tap {x} {y}", MainUi.ClickScreen);
	}

	public static void ClickScreen(Object localDataUncasted)
	{
		ThreadData localData = (ThreadData)localDataUncasted;
		StartAdb(localData);
		localData.done = true;
	}

	private void _OnScreenClicked(InputEvent @event)
	{

		/*if (@event is InputEventMouseButton mouseEvent)
		{
			Vector2 clickCoords = mouseEvent.Position * screenOutput.ReverseScale;
			_ClickScreenAdb((int)clickCoords.X, (int)clickCoords.Y);
			GD.Print($"Mouse Event Position : {clickCoords}");
			
		}*/
	}
}





