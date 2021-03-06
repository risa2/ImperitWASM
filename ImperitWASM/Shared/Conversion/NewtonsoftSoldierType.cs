﻿using System;
using ImperitWASM.Shared.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImperitWASM.Shared.Conversion
{
	public class NewtonsoftSoldierType : JsonConverter<SoldierType>
	{
		public override SoldierType ReadJson(JsonReader reader, Type objectType, SoldierType? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var e = JObject.Load(reader);
			return e["Type"]!.Value<string>() switch
			{
				"E" => new Elephant(e["Description"]!.ToObject<Description>().Must(), e["AttackPower"]!.Value<int>(), e["DefensePower"]!.Value<int>(), e["Weight"]!.Value<int>(), e["Price"]!.Value<int>(), e["Capacity"]!.Value<int>(), e["Speed"]!.Value<int>()),
				"P" => new Pedestrian(e["Description"]!.ToObject<Description>().Must(), e["AttackPower"]!.Value<int>(), e["DefensePower"]!.Value<int>(), e["Weight"]!.Value<int>(), e["Price"]!.Value<int>()),
				"O" => new OutlandishShip(e["Description"]!.ToObject<Description>().Must(), e["AttackPower"]!.Value<int>(), e["DefensePower"]!.Value<int>(), e["Weight"]!.Value<int>(), e["Price"]!.Value<int>(), e["Capacity"]!.Value<int>(), e["Speed"]!.Value<int>()),
				"S" => new Ship(e["Description"]!.ToObject<Description>().Must(), e["AttackPower"]!.Value<int>(), e["DefensePower"]!.Value<int>(), e["Weight"]!.Value<int>(), e["Price"]!.Value<int>(), e["Capacity"]!.Value<int>()),
				_ => throw new JsonException("Unknown type of SoldierType")
			};
		}
		public override void WriteJson(JsonWriter writer, SoldierType? value, JsonSerializer serializer) => throw new NotImplementedException();
	}
}