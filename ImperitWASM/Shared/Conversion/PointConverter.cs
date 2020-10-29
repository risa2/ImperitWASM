using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Conversion
{
	public class PointConverter : JsonConverter<Point>
	{
		public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var parts = reader.GetString().Split("/");
			return new Point(double.Parse(parts[0], CultureInfo.InvariantCulture), double.Parse(parts[1], CultureInfo.InvariantCulture));
		}
		public override void Write(Utf8JsonWriter writer, Point point, JsonSerializerOptions options)
		{
			writer.WriteStringValue(point.x.ToString(CultureInfo.InvariantCulture) + "/" + point.y.ToString(CultureInfo.InvariantCulture));
		}
	}
}