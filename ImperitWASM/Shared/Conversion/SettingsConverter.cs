using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Conversion
{
	public class SettingsConverter : JsonConverter<Settings>
	{
		public override Settings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var root = JsonDocument.ParseValue(ref reader).RootElement;
			return new Settings(root.GetProperty("DebtLimit").GetInt32(), new Ratio(root.GetProperty("DefaultInstability").GetInt32()), root.GetProperty("DefaultMoney").GetInt32(), root.GetProperty("Interest").GetDouble(), Color.Parse(root.GetProperty("LandColor").GetString()), Color.Parse(root.GetProperty("MountainsColor").GetString()), root.GetProperty("MountainsWidth").GetInt32(), Color.Parse(root.GetProperty("SeaColor").GetString()), JsonSerializer.Deserialize<ImmutableArray<SoldierType>>(root.GetProperty("SoldierTypes").GetRawText()), root.GetProperty("FinalLandsCount").GetInt32());
		}
		public override void Write(Utf8JsonWriter writer, Settings s, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("DebtLimit", s.DebtLimit);
			writer.WriteNumber("DefaultInstability", s.DefaultInstability.ToInt());
			writer.WriteNumber("DefaultMoney", s.DefaultMoney);
			writer.WriteNumber("Interest", s.DefaultMoney);
			writer.WriteString("LandColor", s.LandColor.ToString());
			writer.WriteString("MountainsColor", s.MountainsColor.ToString());
			writer.WriteNumber("MountainsWidth", s.MountainsWidth);
			writer.WritePropertyName("SoldierTypes");
			writer.WriteStartArray();
			var stc = new SoldierTypeConverter();
			foreach (var type in s.SoldierTypes)
			{
				stc.Write(writer, type, options);
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
