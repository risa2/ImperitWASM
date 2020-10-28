using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Conversion
{
	public class GraphConverter : JsonConverter<Graph>
	{
		public override Graph Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var array = JsonDocument.ParseValue(ref reader).RootElement.EnumerateArray();
			var table = array.Select(line => line.EnumerateArray().Select(item => item.GetInt32()).ToArray()).ToArray();
			var starts = new int[table.Length + 1];
			starts[0] = 0;
			for (int i = 1; i < starts.Length; ++i)
			{
				starts[i] = starts[i - 1] + table[i - 1].Length;
			}
			return new Graph(table.SelectMany(n => n).ToArray(), starts);
		}
		public override void Write(Utf8JsonWriter writer, Graph value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var line in value)
			{
				writer.WriteStartArray();
				foreach (var item in line)
				{
					writer.WriteNumberValue(item);
				}
				writer.WriteEndArray();
			}
			writer.WriteEndArray();
		}
	}
}
