using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImperitWASM.Shared.Conversion
{
	internal static class NewtonsoftExtensions
	{
		public static T Object<T>(this JToken t, string key) => t[key]!.ToObject<T>() ?? throw new NullReferenceException();
		public static T Value<T>(this JToken t, string key) where T : struct => t[key]?.Value<T>() ?? default;
	}
	public class NewtonsoftSettings : JsonConverter<Settings>
	{
		static Soldiers GetSoldiers(JToken s, ImmutableArray<SoldierType> types)
		{
			return new Soldiers(s.Select(pair => new Regiment(types[pair.Value<int>("Type")], pair.Value<int>("Count"))));
		}
		static ImmutableArray<SoldierType> GetExtraTypes(JToken t, ImmutableArray<SoldierType> types)
		{
			return t.Select(i => types[i.Value<int>()]).ToImmutableArray();
		}
		static Region GetRegion(JToken e, ImmutableArray<SoldierType> types, Ratio instability) => e["Type"]!.Value<int>() switch
		{
			0 => new Land(e["Id"]!.Value<int>(), e["Name"]!.Value<string>(), e["Shape"]!.ToObject<Shape>().Must(), GetSoldiers(e["Soldiers"].Must(), types), GetExtraTypes(e["ExtraTypes"].Must(), types), e["Earnings"]!.Value<int>(), instability, e.Value<bool>("IsStart"), e.Value<bool>("IsFinal"), e.Value<bool>("HasPort")),
			1 => new Sea(e["Id"]!.Value<int>(), e["Name"]!.Value<string>(), e["Shape"]!.ToObject<Shape>().Must(), GetSoldiers(e["Soldiers"].Must(), types), GetExtraTypes(e["ExtraTypes"].Must(), types)),
			2 => new Mountains(e["Id"]!.Value<int>(), e["Name"]!.Value<string>(), e["Shape"]!.ToObject<Shape>().Must()),
			_ => throw new JsonException("Unknown type of Region")
		};
		public override Settings ReadJson(JsonReader reader, Type objectType, Settings? existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var js = JObject.Load(reader);
			var types = js["SoldierTypes"]!.Select(type => type.ToObject<SoldierType>().Must()).ToImmutableArray();
			var instability = js.Object<Ratio>("DefaultInstability");
			var regions = js["Regions"]!.Select(r => GetRegion(r, types, instability)).ToImmutableArray();
			var names = js["Names"]!.Select(name => name.Value<string>()).ToImmutableArray();
			return new Settings(js.Value<int>("CountdownSeconds"), instability, js.Value<int>("DebtLimit"), js.Value<int>("DefaultMoney"), js.Value<int>("FinalLandsCount"), js.Object<Graph>("Graph"), js.Object<Ratio>("Interest"), js.Object<Color>("LandColor"), js.Value<int>("PlayerCount"), js.Object<Color>("MountainsColor"), js.Value<int>("MountainsWidth"), names, regions, js.Object<Color>("SeaColor"), types);
		}
		public override void WriteJson(JsonWriter writer, Settings? value, JsonSerializer serializer) => throw new NotImplementedException();
	}
}
