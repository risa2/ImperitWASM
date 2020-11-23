using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class GraphConverter : JsonConverter<Graph>
	{
		public override Graph Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var array = JsonDocument.ParseValue(ref reader).RootElement.EnumerateArray();
			var table = array.Select(line => line.EnumerateArray().Select(item => item.GetInt32()).ToList()).ToList();
			var starts = ImmutableArray.CreateBuilder<int>(table.Count + 1);
			starts[0] = 0;
			for (int i = 1; i < starts.Capacity; ++i)
			{
				starts[i] = starts[i - 1] + table[i - 1].Count;
			}
			return new Graph(table.SelectMany(n => n).ToImmutableArray(), starts.MoveToImmutable());
		}
		public override void Write(Utf8JsonWriter writer, Graph value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var line in value)
			{
				writer.WriteStartArray();
				foreach (int item in line)
				{
					writer.WriteNumberValue(item);
				}
				writer.WriteEndArray();
			}
			writer.WriteEndArray();
		}
	}
}
