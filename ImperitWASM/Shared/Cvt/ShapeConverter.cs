using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class ShapeConverter : JsonConverter<Shape>
	{
		public override Shape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var points = JsonDocument.ParseValue(ref reader).RootElement;
			return new Shape(points.GetProperty("Border").EnumerateArray().Select(p => Point.Parse(p.GetString().Must())).ToImmutableArray(), Point.Parse(points.GetProperty("Center").GetString().Must()));
		}
		public override void Write(Utf8JsonWriter writer, Shape shape, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString("Center", shape.Center.ToString());
			writer.WriteStartArray();
			foreach (var point in shape)
			{
				writer.WriteStringValue(point.ToString());
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
