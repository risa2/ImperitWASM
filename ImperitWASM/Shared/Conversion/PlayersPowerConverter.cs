using ImperitWASM.Shared.State;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.Conversion
{
	public class PlayersPowerConverter : JsonConverter<PlayersPower>
	{
		static PlayerPower Parse(JsonElement e)
		{
			return new PlayerPower(e.GetProperty("Alive").GetBoolean(), e.GetProperty("Soldiers").GetInt32(), e.GetProperty("Lands").GetInt32(), e.GetProperty("Income").GetInt32(), e.GetProperty("Money").GetInt32());
		}
		public override PlayersPower Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var array = JsonDocument.ParseValue(ref reader).RootElement.EnumerateArray();
			return new PlayersPower(array.Select(p => Parse(p)).ToImmutableArray());
		}
		public override void Write(Utf8JsonWriter writer, PlayersPower pps, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var pp in pps)
			{
				writer.WriteStartObject();
				writer.WriteBoolean("Alive", pp.Alive);
				writer.WriteNumber("Income", pp.Income);
				writer.WriteNumber("Lands", pp.Lands);
				writer.WriteNumber("Money", pp.Money);
				writer.WriteNumber("Soldiers", pp.Soldiers);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}
	}
}