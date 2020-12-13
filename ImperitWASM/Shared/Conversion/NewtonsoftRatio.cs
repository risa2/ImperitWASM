using System;
using ImperitWASM.Shared.Data;
using Newtonsoft.Json;

namespace ImperitWASM.Shared.Conversion
{
	public class NewtonsoftRatio : JsonConverter<Ratio>
	{
		public override Ratio ReadJson(JsonReader reader, Type objectType, Ratio existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return new Ratio(serializer.Deserialize<int>(reader));
		}
		public override void WriteJson(JsonWriter writer, Ratio value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value.ToInt());
		}
	}
}
