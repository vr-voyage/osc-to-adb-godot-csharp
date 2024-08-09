using Godot;
using Godot.Collections;

namespace AdbGodotSharp
{
    public static class JsonExtensions
    {
        /*public static string ToJson(this Vector4 vec4)
        {
            return Json.Stringify(new Godot.Collections.Array<float>{ vec4.X, vec4.Y, vec4.Z, vec4.W});
        }

        public static string ToJson(this Godot.Color color)
        {
            Vector4 colorVectorized = new Vector4( color.R, color.G, color.B, color.A );
            return colorVectorized.ToJson();
        }

        public static string ToJson(this Godot.Vector2 vec2)
        {
            return Json.Stringify(new Godot.Collections.Array<float> { vec2.X, vec2.Y });
        }*/
    }

    public static class SerializationHelpers
    {
        public static Array<float> Serialize(this Color color)
        {
            return new Array<float>{ color.R, color.G, color.B, color.A};
        }

        public static Array<float> Serialize(this Vector2 vec2)
        {
            return new Array<float> { vec2.X, vec2.Y };
        }

        public static bool AllComponentsAre(this Array<Variant> variantArray, Variant.Type checkedType)
        {
            bool correct = true;
            foreach (var element in variantArray)
            {
                correct &= (element.VariantType == checkedType);
            }
            return correct;
        }

        public static Color ToColor(this Array<Variant> serializedColor)
        {
            if (serializedColor.Count == 4 && serializedColor.AllComponentsAre(Variant.Type.Float))
            {
                Color color = new Color(
                    (float)serializedColor[0],
                    (float)serializedColor[1],
                    (float)serializedColor[2],
                    (float)serializedColor[3]);
                return color;
            }

            return Colors.Black;
        }

        public static Vector2 ToVec2(this Array<Variant> serializedVec2)
        {
            if (serializedVec2.Count == 2 && serializedVec2.AllComponentsAre(Variant.Type.Float))
            {
                return new Vector2((float)serializedVec2[0], (float)serializedVec2[1]);
            }
            return Vector2.Zero;
        }
    }


    public static class VariantHelpers
    {
        public static float ToFloat(this Godot.Variant variant)
        {
            return variant.VariantType switch
            {
                Variant.Type.Int => (int)variant,
                Variant.Type.Float => (float)variant,
                Variant.Type.Bool => ((bool)variant ? 1f : 0f),
                _ => 0f,
            };
        }

        
    }

    public static class DictionaryHelpers
    {
        public static bool HasKeys(this Dictionary<string, Variant> dictionary, params string[] keys)
        {
            if (keys.IsEmpty()) return false;
            bool hasAllKeys = true;
            foreach (string key in keys)
            {
                hasAllKeys &= dictionary.ContainsKey(key);
            }
            return hasAllKeys;
        }

        public static bool HasFormat(
            this Dictionary<string, Variant> dictionary,
            params (string key, Variant.Type valueType)[] values)
        {
            if (values.Length == 0) return false;
            bool allValuesHaveCorrectFormat = true;
            foreach ((string key, Variant.Type valueType) in values)
            {
                allValuesHaveCorrectFormat &=
                    (dictionary.HasKeys(key) && dictionary[key].VariantType == valueType);
            }
            return allValuesHaveCorrectFormat;
        }
    }
}
