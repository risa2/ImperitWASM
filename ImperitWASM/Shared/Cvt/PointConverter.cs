using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Cvt
{
	public class PointConverter : JsonConverter<Point>
	{
		public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return Point.Parse(reader.GetString());
		}
		public override void Write(Utf8JsonWriter writer, Point point, JsonSerializerOptions options)
		{
			writer.WriteStringValue(point.ToString());
		}
	}
}