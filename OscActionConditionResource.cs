using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace AdbGodotSharp
{
	public enum OscActionConditionEnum
	{
		Equals,
		Different,
		GreaterOrEqual,
		LowerOrEqual
	}

	public static class EnumHelpers
	{
		public static string ToReadableString(this OscActionConditionEnum condition)
		{
			return condition switch
			{
				OscActionConditionEnum.Equals => "=",
				OscActionConditionEnum.Different => "≠",
				OscActionConditionEnum.GreaterOrEqual => "≧",
				OscActionConditionEnum.LowerOrEqual => "≦",
				_ => ""
			};
		}
	}

	public static class VariantHelpers
	{
		public static float ToFloat(this Godot.Variant variant)
		{
			return variant.VariantType switch
			{
				Variant.Type.Int   => (int)variant,
				Variant.Type.Float => (float)variant,
				Variant.Type.Bool  => ((bool)variant ? 1f : 0f),
				_ => 0f,
			};
		}
	}

	public partial class OscActionConditionResource : Resource
	{
		[Export]
		public string Path { get; set; }

		[Export]
		public OscActionConditionEnum Condition { get; set; } = OscActionConditionEnum.Equals;

		[Export]
		public float Threshold { get; set; }

		public bool LastCheckResult { get; set; } = false;

		delegate bool ConditionChecker(float value, float threshold);

		Dictionary<OscActionConditionEnum, ConditionChecker> checkers =
			new Dictionary<OscActionConditionEnum, ConditionChecker>
			{
				{ 
					OscActionConditionEnum.Equals,
					(float a, float b) => { return a == b; }
				},
				{ 
					OscActionConditionEnum.Different,
					(float a, float b) => { return a != b; }
				},
				{ 
					OscActionConditionEnum.GreaterOrEqual,
					(float a, float b) => { return a >= b; }
				},
				{ 
					OscActionConditionEnum.LowerOrEqual,
					(float a, float b) => { return a <= b; }
				}
			};

		public bool IsMetWith(float value)
		{
			bool checkResult = checkers[Condition](value, Threshold);
			LastCheckResult = checkResult;
			return checkResult;
		}

	}
}
