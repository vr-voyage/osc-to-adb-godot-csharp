using AdbGodotSharp;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

public partial class AdbHandler : Control
{

	[Export]
	public UserMainSettingsResource MainSettings { get; set; }

	[Export]
	public ScaledTextureRect screenOutput;

	[Export]
	public StatusBar StatusBar { get; set; }

	public string SettingsFilePath { get; set; } = "user://MainSettings.res";

	public enum ThreadType
	{
		Unknown,
		AdbScreenshot,
		AdbClick
	};

	public class ThreadData
	{
		public Thread threadHandle;
		public ThreadType threadType = ThreadType.Unknown;
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

	public static void TakeScreenshot(object localDataUncasted)
	{
		GD.Print("Taking screenshot !");
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

	public override void _Process(double delta)
	{
		if (currentThreadData == null || currentThreadData.done == false)
		{
			return;
		}
		
		if (currentThreadData.threadType == ThreadType.AdbScreenshot)
		{
			ShowStatus("Showing screenshot");
			_ShowScreenshot(currentThreadData.output);
		}
		else
		{
			ShowStatus("Some process finished !");
		}
		currentThreadData = null;
	}

	public void _ShowScreenshot(byte[] screenshotData)
	{
		ShowStatus(screenshotData.Length.ToString());
		var image = new Image();
		Godot.Error ret = image.LoadPngFromBuffer(screenshotData);
		if (ret != Error.Ok)
		{
			ShowStatus("Could not load image !");
			return;
		}
		screenOutput.ApplyAndScaleTexture(ImageTexture.CreateFromImage(image));
	}

	private void _InvokeAdb(
		string arguments,
		ParameterizedThreadStart handler,
		ThreadType threadType = ThreadType.AdbScreenshot)
	{
		if (MainSettings == null)
		{
			ShowStatus("No useable settings !");
			return;
		}

		string adbPath = MainSettings.adbPath;

		if (adbPath == null || adbPath.Trim().Length == 0)
		{
			ShowStatus("ADB Path is not set !");
			return;
		}

		currentThreadData = new ThreadData();
		currentThreadData.adbExeFilePath = adbPath;
		currentThreadData.arguments = arguments;
		currentThreadData.threadType = threadType;
		Thread newThread = new Thread(handler);
		newThread.Start(currentThreadData);
	}

	private void _TriggerScreenshotAdb()
	{
		_InvokeAdb("exec-out screencap -p", AdbHandler.TakeScreenshot, ThreadType.AdbScreenshot);
	}

	public void ClickTriggered(Vector2 screenPosition)
	{
		_ClickScreenAdb((int)screenPosition.X, (int)screenPosition.Y);
	}

	private void _ClickScreenAdb(int x, int y)
	{
		_InvokeAdb($"shell input tap {x} {y}", AdbHandler.ClickScreen, ThreadType.AdbClick);
	}

	public static void ClickScreen(Object localDataUncasted)
	{
		ThreadData localData = (ThreadData)localDataUncasted;
		StartAdb(localData);
		localData.done = true;
	}

	void ShowStatus(string message)
	{
		StatusBar?.ShowMessage(message);
	}

	public void SetSettings(UserMainSettingsResource settings)
	{
		if (settings != null)
		{
			MainSettings = settings;
			SetProcess(true);
			ShowStatus("ADB configuration provided");
		}
		else
		{
			SetProcess(false);
			ShowStatus("No useable configuration for ADB");
		}
	}

	public override void _Ready()
	{
		if (MainSettings == null)
		{
			SetProcess(false);
			ShowStatus("No useable configuration for ADB");
		}
	}

	void KillCurrentProcess()
	{
		currentThreadData?.currentProcess?.Kill();
	}

	void KillAnyAdb()
	{
		foreach (var process in Process.GetProcessesByName("adb"))
		{
			process.Kill();
		}
	}

	void Termination()
	{
		KillCurrentProcess();
		KillAnyAdb();

	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			Termination();
		}
	}

	public override void _ExitTree()
	{
		Termination();
	}

}

