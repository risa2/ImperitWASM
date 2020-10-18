using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;
using System;
using System.Threading.Tasks;

namespace ImperitWASM.Server.Services
{
	public interface IGameLoader
	{
		Task Register();
		bool IsActive { get; }
		Task Start();
		Task Finish();
		TimeSpan TimeSinceFirstRegistration { get; }
	}
	public class GameLoader : IGameLoader
	{
		readonly JsonWriter<JsonGame, Game, Settings> writer;
		Game game;
		public GameLoader(IServiceIO io, ISettingsLoader sl)
		{
			writer = new JsonWriter<JsonGame, Game, Settings>(io.Game, sl.Settings, JsonGame.From);
			game = writer.LoadOne();
		}
		public bool IsActive => game.IsActive;
		public Task Start()
		{
			game = Game.Start();
			return writer.Save(game);
		}
		public Task Finish()
		{
			game = Game.Finish();
			return writer.Save(game);
		}
		public TimeSpan TimeSinceFirstRegistration => DateTime.UtcNow.Subtract(game.FirstRegistration);
		public Task Register()
		{
			game = game.Register();
			return writer.Save(game);
		}
	}
}
