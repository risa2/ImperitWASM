using System.IO;
using Newtonsoft.Json;
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
		public Config(params string[] parts) => Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(parts))).Must();
	}
}
