using ImperitWASM.Shared.State;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.Conversion
{
	public class PointConverter : JsonConverter<Point>
	{
		public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var parts = reader.GetString().Split("/");
			return new Point(double.Parse(parts[0], ExtMethods.Culture), double.Parse(parts[1], ExtMethods.Culture));
		}
		public override void Write(Utf8JsonWriter writer, Point point, JsonSerializerOptions options)
		{
			writer.WriteStringValue(point.x.ToString(ExtMethods.Culture) + "/" + point.y.ToString(ExtMethods.Culture));
		}
	}
}