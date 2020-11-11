using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class ProvinceDataConverter : JsonConverter<ProvinceData>
	{
		public override ProvinceData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var root = JsonDocument.ParseValue(ref reader).RootElement;
			bool has_earnings = root.TryGetProperty("Earnings", out var earnings);
			bool has_isStart = root.TryGetProperty("IsStart", out var isStart);
			bool has_isFinal = root.TryGetProperty("IsFinal", out var isFinal);
			bool has_hasPort = root.TryGetProperty("HasPort", out var hasPort);
			return new ProvinceData(root.GetProperty("Name").GetString().Must(), Enum.Parse<ProvinceData.Kind>(root.GetProperty("Type").GetString()), JsonSerializer.Deserialize<Shape>(root.GetProperty("Shape").GetRawText()).Must(), JsonSerializer.Deserialize<ImmutableArray<Tuple<int, int>>>(root.GetProperty("Soldiers").GetRawText()), JsonSerializer.Deserialize<ImmutableArray<int>>(root.GetProperty("ExtraTypes").GetRawText()), has_earnings ? (int?)earnings.GetInt32() : null, has_isStart ? (bool?)isStart.GetBoolean() : null, has_isFinal ? (bool?)isFinal.GetBoolean() : null, has_hasPort ? (bool?)hasPort.GetBoolean() : null);
		}
		public override void Write(Utf8JsonWriter writer, ProvinceData value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString("Name", value.Name);
			writer.WriteString("Type", value.Type.ToString());
			writer.WritePropertyName("Shape");
			var sc = new ShapeConverter();
			sc.Write(writer, value.Shape, options);
			writer.WritePropertyName("Soldiers");
			writer.WriteStartArray();
			foreach (var pair in value.Soldiers)
			{
				writer.WriteNumber("Item1", pair.Item1);
				writer.WriteNumber("Item2", pair.Item2);
			}
			writer.WriteEndArray();
			writer.WritePropertyName("ExtraTypes");
			writer.WriteStartArray();
			value.ExtraTypes.Each(writer.WriteNumberValue);
			writer.WriteEndArray();
			if (value.Earnings is int earnings)
			{
				writer.WriteNumber("Earnings", earnings);
			}
			if (value.IsFinal is bool isFinal)
			{
				writer.WriteBoolean("IsFinal", isFinal);
			}
			if (value.IsStart is bool isStart)
			{
				writer.WriteBoolean("IsStart", isStart);
			}
			if (value.HasPort is bool hasPort)
			{
				writer.WriteBoolean("HasPort", hasPort);
			}
			writer.WriteEndObject();
		}
	}
}
