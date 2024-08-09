using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdbGodotSharp
{
	public partial class UserDefinedLocationClickerResource : Resource
	{
		[Export]
		public bool Enabled { get; set; }

		[Export]
		public OscActionConditionResource Condition { get; set; } = new OscActionConditionResource();

		[Export]
		public Color Color { get; set; } = RandomColor();

		[Export]
		public Vector2 Position { get; set; }

		public static Color RandomColor()
		{
			var random = new Random();
			var randomColor = new Color((uint)random.Next());
			randomColor.A = 1f;

			return randomColor;
        }

        public Godot.Collections.Dictionary<string, Variant> Serialize()
		{
			var manualSerialization = new Godot.Collections.Dictionary<string, Variant>()
			{
				{ "Enabled", Enabled },
				{ "Condition", Condition.Serialize() },
				{ "Color", Color.Serialize() },
				{ "Position", Position.Serialize() }
			};
			return manualSerialization;
		}

		public static UserDefinedLocationClickerResource Deserialize(Godot.Collections.Dictionary<string, Variant> serializedData)
		{
			bool formatCheck = serializedData.HasFormat(
				("Enabled", Variant.Type.Bool),
				("Condition", Variant.Type.Dictionary),
				("Color", Variant.Type.Array),
				("Position", Variant.Type.Array));
			if (!formatCheck) return null;

			var serializedCondition = (Godot.Collections.Dictionary<string, Variant>)serializedData["Condition"];
			OscActionConditionResource actionCondition = OscActionConditionResource.Deserialize(serializedCondition);
			if (actionCondition == null)
			{
				return null;
			}

			return new UserDefinedLocationClickerResource()
			{
				Enabled = (bool)serializedData["Enabled"],
				Condition = actionCondition,
				Color = ((Array<Variant>)serializedData["Color"]).ToColor(),
				Position = ((Array<Variant>)serializedData["Position"]).ToVec2()
			};
		}
    }
}
