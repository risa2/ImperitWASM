using System;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IGameLoader
	{
		Task StartCountdown();
		bool IsActive { get; }
		Task Start();
		Task Finish();
		TimeSpan TimeToStart { get; }
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
		public Task Start() => writer.Save(game = Game.Start());
		public Task Finish() => writer.Save(game = Game.Finish());
		public Task StartCountdown() => writer.Save(game = game.StartCountdown());
		public TimeSpan TimeToStart => TimeSpan.FromMinutes(3) - game.TimeSinceCountdownStart;
	}
}
