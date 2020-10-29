using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;
using ImperitWASM.Shared.State.SoldierTypes;

namespace ImperitWASM.Shared.Conversion
{
	public class SoldierTypeConverter : JsonConverter<SoldierType>
	{
		public override SoldierType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var e = JsonDocument.ParseValue(ref reader).RootElement;
			return e.GetProperty("Type").GetString() switch
			{
				"E" => new Elephant(e.GetProperty("Id").GetInt32(), JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32(), e.GetProperty("Speed").GetInt32(), e.GetProperty("RecruitPlaces").EnumerateArray().Select(x => x.GetInt32()).ToImmutableArray()),
				"ES" => new ElephantShip(e.GetProperty("Id").GetInt32(), JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32(), e.GetProperty("Speed").GetInt32(), e.GetProperty("RecruitPlaces").EnumerateArray().Select(x => x.GetInt32()).ToImmutableArray()),
				"P" => new Pedestrian(e.GetProperty("Id").GetInt32(), JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32()),
				"S" => new Ship(e.GetProperty("Id").GetInt32(), JsonSerializer.Deserialize<Description>(e.GetProperty("Description").GetRawText()), e.GetProperty("AttackPower").GetInt32(), e.GetProperty("DefensePower").GetInt32(), e.GetProperty("Weight").GetInt32(), e.GetProperty("Price").GetInt32(), e.GetProperty("Capacity").GetInt32()),
				_ => throw new JsonException("Unknown type of SoldierType")
			};
		}
		public override void Write(Utf8JsonWriter writer, SoldierType t, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("Id", t.Id);
			writer.WriteString("Type", t is Elephant ? "E" : t is ElephantShip ? "ES" : t is Ship ? "S" : "P");
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
				writer.WriteStartArray();
				foreach (int place in elephant.RecruitPlaces)
				{
					writer.WriteNumberValue(place);
				}
				writer.WriteEndArray();
			}
			else if (t is Ship ship)
			{
				writer.WriteNumber("Capacity", ship.Capacity);
				if (t is ElephantShip eship)
				{
					writer.WriteNumber("Speed", eship.Speed);
					writer.WriteStartArray();
					foreach (int place in eship.RecruitPlaces)
					{
						writer.WriteNumberValue(place);
					}
					writer.WriteEndArray();
				}
			}
			writer.WriteEndObject();
		}
	}
}