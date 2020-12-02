using System.IO;
using Newtonsoft.Json;
using ImperitWASM.Shared;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public static class Config
	{
		public static Settings Load(params string[] parts)
		{
			return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(parts))).Must();
		}
	}
}
