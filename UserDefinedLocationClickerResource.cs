using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	}
}
