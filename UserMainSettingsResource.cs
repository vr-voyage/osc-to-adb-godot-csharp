using System;
using Godot;

namespace AdbGodotSharp
{
	[GlobalClass]
	public partial class UserMainSettingsResource : Resource
	{
		
		[Export(PropertyHint.GlobalFile, "*.exe")]
		public string adbPath = "C:/Program Files/Unity/Hub/Editor/2022.3.21f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe";

		private const string serializedType = "Voyage.UserMainSettingsResource";


        public bool AdbAppearUseable()
		{
			GD.Print($"Provided AdbPath is {adbPath}");
			return FileAccess.FileExists(adbPath);
		}

		public Godot.Collections.Dictionary<string, Variant> Serialize()
		{
			return new Godot.Collections.Dictionary<string, Variant>()
			{
				{ "@type", serializedType },
				{ "version", 1 },
				{ "adbPath", adbPath }
			};
		}

		private static UserMainSettingsResource Deserialize(
			Godot.Collections.Dictionary<string, Variant> deserializedData)
		{
			bool formatCheck = deserializedData.HasFormat(
				("@type", Variant.Type.String),
				("version", Variant.Type.Float),
				("adbPath", Variant.Type.String));

			if (!formatCheck)
			{
				GD.Print("Not our data");
				return null;
			}

			string type = (string)deserializedData["@type"];
			if (type != serializedType)
			{
				GD.Print($"Wrong type. Expected {serializedType} got {type}");
				return null;
			}

			var returnedResource = new UserMainSettingsResource();
			returnedResource.adbPath = (string)deserializedData["adbPath"];


			return returnedResource;
        }

		public static UserMainSettingsResource FromJson(string jsonData)
		{
			var parsedData = Json.ParseString(jsonData);
			if (parsedData.VariantType != Variant.Type.Dictionary)
			{
				GD.Print("I don't know what that is");
				return null;
			}

			return Deserialize((Godot.Collections.Dictionary<string, Variant>)parsedData);
		}

		public string ToJson()
		{
			return Json.Stringify(Serialize());
		}
    }
}
