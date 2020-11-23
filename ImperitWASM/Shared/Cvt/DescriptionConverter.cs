using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class DescriptionConverter : JsonConverter<Description>
	{
		public override Description Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var doc = JsonDocument.ParseValue(ref reader).RootElement;
			return new Description(doc.GetProperty("Name").GetString(), doc.GetProperty("Text").EnumerateArray().Select(line => line.GetString().Must()).ToImmutableArray(), doc.GetProperty("Symbol").GetString());
		}
		public override void Write(Utf8JsonWriter writer, Description d, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString("Name", d.Name);
			writer.WriteString("Symbol", d.Symbol);
			writer.WritePropertyName("Text");
			writer.WriteStartArray();
			d.Text.Each(writer.WriteStringValue);
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}