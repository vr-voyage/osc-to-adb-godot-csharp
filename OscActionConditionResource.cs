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


		public Godot.Collections.Dictionary<string, Variant> Serialize()
		{
			var manualSerialization = new Godot.Collections.Dictionary<string, Variant>()
			{
				{ "Path", Path },
				{ "Condition", (int)Condition },
				{ "Threshold", Threshold }
			};
			return manualSerialization;
		}

		public static OscActionConditionResource Deserialize(Godot.Collections.Dictionary<string, Variant> serializedData)
		{
			bool formatCheck = serializedData.HasFormat(
				("Path", Variant.Type.String),
				("Condition", Variant.Type.Float),
				("Threshold", Variant.Type.Float));
			if (!formatCheck) return null;

			int jsonedCondition = (int)(float)serializedData["Condition"];
			if (jsonedCondition < 0 || jsonedCondition >= 4)
			{
				GD.PrintErr($"Invalid condition {jsonedCondition}");
				return null;
			}

			return new OscActionConditionResource()
			{
				Path = (string)serializedData["Path"],
				Condition = (OscActionConditionEnum)jsonedCondition,
				Threshold = (float)serializedData["Threshold"]
			};
		}
	}
}
