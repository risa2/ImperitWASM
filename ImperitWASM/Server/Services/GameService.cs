using System;
using System.Linq;
using System.Linq.Expressions;
using ImperitWASM.Server.Load;

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
		int? RegistrableGame { get; }
	}
	public class GameService : IGameService
	{
		private readonly IContextService ctx;
		private readonly IConfig cfg;
		public GameService(IContextService ctx, IConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
		}
		public int Create() => ctx.Games.Add(Game.Create).Entity.Id;
		private static Expression<Func<Game, bool>> TimeElapsed(Game.State state, DateTime deadline) => g => g.Current == state && g.LastChange >= deadline;
		private static Expression<Func<Game, bool>> InState(Game.State state) => g => g.Current == state;
		private static Expression<Func<Game, bool>> InState(Game.State a, Game.State b) => g => g.Current == a || g.Current == b;
		public void RemoveOld(TimeSpan period) => ctx.Games.RemoveAt(TimeElapsed(Game.State.Finished, DateTime.UtcNow - period));
		public void Start(int gameId) => ctx.Games.UpdateAt(gameId, g => g.Start());
		public void Finish(int gameId) => ctx.Games.UpdateAt(gameId, g => g.Finish());
		public void StartCountdown(int gameId) => ctx.Games.UpdateAt(gameId, g => g.StartCountdown());
		public bool Started(int gameId) => ctx.Games.Find(gameId).Started;
		public bool Finished(int gameId) => ctx.Games.Find(gameId).Finished;
		public int[] ShouldStart => ctx.Games.Where(TimeElapsed(Game.State.Countdown, DateTime.UtcNow - cfg.Settings.CountdownTime)).Select(g => g.Id).ToArray();
		public int[] StartedGames => ctx.Games.Where(InState(Game.State.Started)).Select(g => g.Id).ToArray();
		public int[] FinishedGames => ctx.Games.Where(InState(Game.State.Finished)).Select(g => g.Id).ToArray();
		public int? RegistrableGame => ctx.Games.FirstOrDefault(InState(Game.State.Countdown, Game.State.Created))?.Id;
	}
}