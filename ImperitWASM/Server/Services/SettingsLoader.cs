using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface ISettingsLoader
	{
		Settings Settings { get; }
	}
	public class SettingsLoader : ISettingsLoader
	{
		public Settings Settings { get; }
		public SettingsLoader(IServiceIO io)
		{
			Settings = new JsonWriter<JsonSettings, Settings, bool>(io.Settings, false, JsonSettings.From).LoadOne();
		}
	}
}