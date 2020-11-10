using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Shared.Cvt
{
	public class ProvinceFactoryConverter : JsonConverter<ProvinceFactory>
	{
		public override ProvinceFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException(); 
		}
		public override void Write(Utf8JsonWriter writer, ProvinceFactory value, JsonSerializerOptions options)
		{

		}
	}
}
