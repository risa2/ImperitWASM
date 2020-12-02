using System.IO;
using static Newtonsoft.Json.JsonConvert;
using ImperitWASM.Shared;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IConfig
	{
		Settings Settings { get; }
	}
	public class Config : IConfig
	{
		public Settings Settings { get; }
		public Config(params string[] parts) => Settings = DeserializeObject<Settings>(File.ReadAllText(Path.Combine(parts))).Must();
	}
}
