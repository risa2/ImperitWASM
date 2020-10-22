using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Conversion
{
	public class ProbabilityConverter : JsonConverter<Probability>
	{
		public override Probability Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new Probability(reader.GetInt32());
		public override void Write(Utf8JsonWriter writer, Probability p, JsonSerializerOptions options) => writer.WriteNumberValue(p.ToUnits(int.MaxValue));
	}
}
