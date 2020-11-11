using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class SoldierTypeConverter : JsonConverter<SoldierType>
	{
		public override SoldierType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var e = JsonDocument.ParseValue(ref reader).RootElement;
			return e.GetProperty("Type").GetString() switch
			{
				"E" => new Elephant(JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()).Must(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32(), e.GetProperty("Speed").GetInt32()),
				"P" => new Pedestrian(JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()).Must(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32()),
				"O" => new OutlandishShip(JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()).Must(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32(), e.GetProperty("Speed").GetInt32()),
				"S" => new Ship(JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()).Must(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32()),
				_ => throw new JsonException("Unknown type of SoldierType")
			};
		}
		public override void Write(Utf8JsonWriter writer, SoldierType t, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString("Type", t is Elephant ? "E" : t is OutlandishShip ? "ES" : t is Ship ? "S" : "P");
			writer.WritePropertyName("Description");
			new DescriptionConverter().Write(writer, t.Description, options);
			writer.WriteNumber("AttackPower", t.AttackPower);
			writer.WriteNumber("DefensePower", t.DefensePower);
			writer.WriteNumber("Price", t.Price);
			writer.WriteNumber("Weight", t.Weight);
			if (t is Elephant elephant)
			{
				writer.WriteNumber("Capacity", elephant.Capacity);
				writer.WriteNumber("Speed", elephant.Speed);
			}
			else if (t is Ship ship)
			{
				writer.WriteNumber("Capacity", ship.Capacity);
				if (t is OutlandishShip os)
				{
					writer.WriteNumber("Speed", os.Speed);
				}
			}
			writer.WriteEndObject();
		}
	}
}