using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdbGodotSharp
{
    public partial class OscValuesRead : Resource
    {
        [Export]
        public Godot.Collections.Dictionary<string, Variant> Values { get; set; } = new Godot.Collections.Dictionary<string, Variant>();

        public void Set(string name, Variant value)
        {
            Values[name] = value;
        }

        public bool HasValue(string name)
        {
            return Values.ContainsKey(name);
        }

        public float GetValueAsFloat(string name)
        {
            return Values[name].ToFloat();
        }

        public bool IsConditionMet(OscActionConditionResource condition)
        {
            GD.Print("Condition Met");
            string oscPath = condition.Path;
            if (!HasValue(oscPath)) return false;

            float lastReadValue = GetValueAsFloat(oscPath);
            return condition.IsMetWith(lastReadValue);
        }
    }

}
