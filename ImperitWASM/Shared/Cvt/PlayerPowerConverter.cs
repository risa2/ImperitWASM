using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Entities;

namespace ImperitWASM.Shared.Cvt
{
	public class PlayerPowerConverter : JsonConverter<PlayerPower>
	{
		public override PlayerPower Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var s = JsonDocument.ParseValue(ref reader).RootElement.GetString().Split("/");
			return new PlayerPower(int.Parse(s[0]) != 0, int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]), int.Parse(s[4]), int.Parse(s[5]));
		}
		public override void Write(Utf8JsonWriter writer, PlayerPower pp, JsonSerializerOptions options)
		{
			writer.WriteStringValue(string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}/{5}", pp.Alive ? 1 : 0, pp.Income, pp.Lands, pp.Money, pp.Soldiers, pp.Final));
		}
	}
}