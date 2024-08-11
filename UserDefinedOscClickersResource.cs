using Godot;
using Godot.Collections;

namespace AdbGodotSharp
{
	public partial class UserDefinedOscClickersResource : Resource
	{
		[Export]
		public Array<UserDefinedLocationClickerResource> LocationClickers { get; set; } = new Godot.Collections.Array<UserDefinedLocationClickerResource>();

		public Dictionary<string, Variant> Serialize()
		{
			Array<Dictionary<string, Variant>> serializedClickers =
				new();
			foreach (var clicker in LocationClickers)
			{
				serializedClickers.Add(clicker.Serialize());
			}
			Dictionary<string, Variant> serializedValue = new()
			{
				{ "@type", "Voyage.UserDefinedOscClickersResource" },
				{ "version", 1 },
				{ "clickers", serializedClickers }
			};
			return serializedValue;

		}

		public string ToJson()
		{
			return Json.Stringify(Serialize());
		}

		public static UserDefinedOscClickersResource FromJson(string jsonData)
		{
			Dictionary<string, Variant> serializedClickers = 
				JsonHelpers.ParseJsonDictionary(jsonData);
			if (serializedClickers == null)
			{
				GD.PrintErr("I don't know what this is");
				return null;
			}

			if (!serializedClickers.ContainsKey("@type") || !serializedClickers.ContainsKey("clickers"))
			{
				GD.PrintErr("Don't have the required keys");
				return null;
			}

			if (serializedClickers["@type"].VariantType != Variant.Type.String)
			{
				GD.PrintErr("Not our JSON file");
				return null;
			}
			string type = (string)serializedClickers["@type"];
			if (type != "Voyage.UserDefinedOscClickersResource")
			{
				GD.PrintErr("Invalid type");
				return null;
			}

			Variant clickersVariant = serializedClickers["clickers"];
			if (clickersVariant.VariantType != Variant.Type.Array)
			{
				GD.Print("No useable clickers");
				return null;
			}

			Array<Dictionary<string, Variant>> clickers = (Array<Dictionary<string, Variant>>)clickersVariant;

			var clickersResource = new UserDefinedOscClickersResource();
			foreach (var clicker in clickers)
			{
				UserDefinedLocationClickerResource clickerResource = UserDefinedLocationClickerResource.Deserialize(clicker);
				if (clickerResource != null)
				{
					clickersResource.LocationClickers.Add(clickerResource);
				}
			}
			return clickersResource;

		}
	}
}
