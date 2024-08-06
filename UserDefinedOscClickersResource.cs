using Godot;

namespace AdbGodotSharp
{
	public partial class UserDefinedOscClickersResource : Resource
	{
		[Export]
		public Godot.Collections.Array<UserDefinedLocationClickerResource> LocationClickers { get; set; } = new Godot.Collections.Array<UserDefinedLocationClickerResource>();
	}
}
