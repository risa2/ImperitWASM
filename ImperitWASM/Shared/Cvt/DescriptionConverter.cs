using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Cvt
{
	public class DescriptionConverter : JsonConverter<Description>
	{
		public override Description Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var doc = JsonDocument.ParseValue(ref reader).RootElement;
			return new Description(doc.GetProperty("Name").GetString(), doc.GetProperty("Text").GetString(), doc.GetProperty("Symbol").GetString());
		}
		public override void Write(Utf8JsonWriter writer, Description d, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString("Name", d.Name);
			writer.WriteString("Symbol", d.Symbol);
			writer.WriteString("Text", d.Text);
			writer.WriteEndObject();
		}
	}
}