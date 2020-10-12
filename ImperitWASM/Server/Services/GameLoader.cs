using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;
using System;

namespace ImperitWASM.Server.Services
{
	public interface IGameLoader
	{
		void Register();
		bool IsActive { get; }
		void Start();
		void Finish();
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
		public void Start()
		{
			game = Game.Start();
			writer.Save(game);
		}
		public void Finish()
		{
			game = Game.Finish();
			writer.Save(game);
		}
		public TimeSpan TimeSinceFirstRegistration => DateTime.UtcNow.Subtract(game.FirstRegistration);
		public void Register()
		{
			game = game.Register();
			writer.Save(game);
		}
	}
}
