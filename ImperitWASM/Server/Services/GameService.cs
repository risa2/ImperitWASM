using System;
using System.Linq;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Services
{
	public interface IGameService
	{
		int Create();
		void Start(int gameId);
		void Finish(int gameId);
		void RemoveOld(TimeSpan period);
		void StartCountdown(int gameId);
		bool Started(int gameId);
		bool Finished(int gameId);
		int[] StartedGames { get; }
		int[] ShouldStart { get; }
		int[] FinishedGames { get; }
		int RegistrableGame { get; }
	}
	public class GameService : IGameService
	{
		readonly IContextService ctx;
		readonly IConfig cfg;
		public GameService(IContextService ctx, IConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
		}
		public int Create() => ctx.Games.Add(EntityGame.Create).Entity.Id;
		public void RemoveOld(TimeSpan period) => ctx.Games.RemoveAt(g => g.Finished && DateTime.UtcNow - g.LastChange >= period);
		public void Start(int gameId) => ctx.Games.UpdateAt(gameId, g => g.Start());
		public void Finish(int gameId) => ctx.Games.UpdateAt(gameId, g => g.Finish());
		public void StartCountdown(int gameId) => ctx.Games.UpdateAt(gameId, g => g.StartCountdown());
		public bool Started(int gameId) => ctx.Games.Find(gameId).Started;
		public bool Finished(int gameId) => ctx.Games.Find(gameId).Finished;
		public int[] ShouldStart => ctx.Games.Where(g => g.Countdown && cfg.Settings.CountdownElapsed(g.LastChange)).Select(g => g.Id).ToArray();
		public int[] StartedGames => ctx.Games.Where(g => g.Started).Select(g => g.Id).ToArray();
		public int[] FinishedGames => ctx.Games.Where(g => g.Finished).Select(g => g.Id).ToArray();
		public int RegistrableGame => ctx.Games.First(g => g.Countdown || g.Created).Id;
	}
}