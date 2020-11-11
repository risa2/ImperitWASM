using ImperitWASM.Server.Load;
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
		public Config(IFile settings) => Settings = settings.Read<Settings>();
	}
}
