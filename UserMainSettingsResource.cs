using System;
using Godot;

namespace AdbGodotSharp
{
	[GlobalClass]
	public partial class UserMainSettingsResource : Resource
	{
		
		[Export(PropertyHint.GlobalFile, "*.exe")]
		public string adbPath = "file:///Program Files/Unity/Hub/Editor/2022.3.21f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe";

		public bool AdbAppearUseable()
		{
			GD.Print($"Provided AdbPath is {adbPath}");
			return FileAccess.FileExists(adbPath);
		}
	}
}
