using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Conversion
{
	public class RatioConverter : JsonConverter<Ratio>
	{
		public override Ratio Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new Ratio(reader.GetInt32());
		public override void Write(Utf8JsonWriter writer, Ratio p, JsonSerializerOptions options) => writer.WriteNumberValue(p.ToInt());
	}
}
