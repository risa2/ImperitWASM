using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class SettingsConverter : JsonConverter<Settings>
	{
		public override Settings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var root = JsonDocument.ParseValue(ref reader).RootElement;
			return new Settings(root.GetProperty("DebtLimit").GetInt32(), new Ratio(root.GetProperty("DefaultInstability").GetInt32()), root.GetProperty("DefaultMoney").GetInt32(), root.GetProperty("Interest").GetDouble(), Color.Parse(root.GetProperty("LandColor").GetString().Must()), Color.Parse(root.GetProperty("MountainsColor").GetString().Must()), root.GetProperty("MountainsWidth").GetInt32(), Color.Parse(root.GetProperty("SeaColor").GetString().Must()), JsonSerializer.Deserialize<ImmutableArray<SoldierType>>(root.GetProperty("SoldierTypes").GetRawText()), root.GetProperty("FinalLandsCount").GetInt32(), TimeSpan.FromSeconds(root.GetProperty("CountdownTime").GetInt32()), JsonSerializer.Deserialize<Graph>(root.GetProperty("Graph").GetRawText()).Must(), JsonSerializer.Deserialize<ImmutableArray<ProvinceData>>(root.GetProperty("ProvinceData").GetRawText()));
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
			writer.WriteNumber("CountdownTime", (int)s.CountdownTime.TotalSeconds);
			writer.WritePropertyName("SoldierTypes");
			writer.WriteStartArray();
			var stc = new SoldierTypeConverter();
			foreach (var type in s.SoldierTypes)
			{
				stc.Write(writer, type, options);
			}
			writer.WriteEndArray();
			var gc = new GraphConverter();
			gc.Write(writer, s.Graph, options);
			writer.WritePropertyName("ProvinceData");
			writer.WriteStartArray();
			var sc = new ProvinceDataConverter();
			foreach (var data in s.ProvinceData)
			{
				sc.Write(writer, data, options);
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
	}
}
