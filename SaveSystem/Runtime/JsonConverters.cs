using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace antoinegleisberg.Saving
{
    internal class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            JArray array = new(value.r, value.g, value.b, value.a);
            array.WriteTo(writer);
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            Color color = new Color((float)array[0], (float)array[1], (float)array[2], (float)array[3]);
            return color;
        }
    }

    internal class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            JArray array = new(value.x, value.y, value.z);
            array.WriteTo(writer);
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            Vector3 vector = new Vector3((float)array[0], (float)array[1], (float)array[2]);
            return vector;
        }
    }

    internal class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            JArray array = new(value.x, value.y, value.z, value.w);
            array.WriteTo(writer);
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            Quaternion quaternion = new Quaternion((float)array[0], (float)array[1], (float)array[2], (float)array[3]);
            return quaternion;
        }
    }
}
