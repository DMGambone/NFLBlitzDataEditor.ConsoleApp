using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NFLBlitzDataEditor.ConsoleApp
{
    public class ByteArrayConverter
    : JsonConverter
    {
        public override bool CanConvert(Type type)
        {
            if (!type.IsArray)
                return false;

            return (type.GetElementType() == typeof(byte));
        }

        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return true; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IList<byte> buffer = new List<byte>();

            if (reader.TokenType != JsonToken.StartArray)
                throw new InvalidOperationException("Expected a start of an array");

            int? value;
            while ((value = reader.ReadAsInt32()) != null)
                buffer.Add((byte)(value & 0xff));

            return buffer.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<byte> bytes = (IEnumerable<byte>)value;
            writer.WriteStartArray();
            foreach (byte b in bytes)
                writer.WriteValue(b);
            writer.WriteEndArray();
        }
    }

}