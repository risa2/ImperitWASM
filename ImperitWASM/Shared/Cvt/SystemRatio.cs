using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Cvt
{
	public class SystemRatio : JsonConverter<Ratio>
	{
		public override Ratio Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return new Ratio(reader.GetInt32());
		}
		public override void Write(Utf8JsonWriter writer, Ratio p, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(p.ToInt());
		}
	}
}
