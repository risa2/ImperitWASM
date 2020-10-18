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
		static PlayerPower Parse(string[] s)
		{
			return new PlayerPower(int.Parse(s[0]) != 0, int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]), int.Parse(s[4]));
		}
		public override PlayersPower Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var array = JsonDocument.ParseValue(ref reader).RootElement.EnumerateArray();
			return new PlayersPower(array.Select(p => Parse(p.GetString().Split("/"))).ToImmutableArray());
		}
		public override void Write(Utf8JsonWriter writer, PlayersPower pps, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var pp in pps)
			{
				writer.WriteStringValue(string.Format(ExtMethods.Culture, "{0}/{1}/{2}/{3}/{4}", pp.Alive ? 1 : 0, pp.Income, pp.Lands, pp.Money, pp.Soldiers));
			}
			writer.WriteEndArray();
		}
	}
}