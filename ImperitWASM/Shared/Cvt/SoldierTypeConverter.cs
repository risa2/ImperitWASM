using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Entities;

namespace ImperitWASM.Shared.Cvt
{
	public class SoldierTypeConverter : JsonConverter<SoldierType>
	{
		public override SoldierType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var e = JsonDocument.ParseValue(ref reader).RootElement;
			return e.GetProperty("Type").GetString() switch
			{
				"E" => new Elephant(e.GetProperty("Name").GetString(), e.GetProperty("Text").GetString(), e.GetProperty("Symbol").GetString(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32(), e.GetProperty("Speed").GetInt32()),
				"P" => new Pedestrian(e.GetProperty("Name").GetString(), e.GetProperty("Text").GetString(), e.GetProperty("Symbol").GetString(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32()),
				"O" => new OutlandishShip(e.GetProperty("Name").GetString(), e.GetProperty("Text").GetString(), e.GetProperty("Symbol").GetString(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32(), e.GetProperty("Speed").GetInt32()),
				"S" => new Ship(e.GetProperty("Name").GetString(), e.GetProperty("Text").GetString(), e.GetProperty("Symbol").GetString(), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32()),
				_ => throw new JsonException("Unknown type of SoldierType")
			};
		}
		public override void Write(Utf8JsonWriter writer, SoldierType t, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteString("Type", t is Elephant ? "E" : t is OutlandishShip ? "ES" : t is Ship ? "S" : "P");
			writer.WriteString("Name", t.Name);
			writer.WriteString("Text", t.Text);
			writer.WriteString("Symbol", t.Symbol);
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