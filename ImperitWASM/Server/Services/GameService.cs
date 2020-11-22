using System;
using System.Linq;
using System.Linq.Expressions;
using ImperitWASM.Server.Load;

namespace ImperitWASM.Server.Services
{
	public interface IGameService
	{
		Game Find(int gameId);
		void Finish(int gameId);
		void RemoveOld(TimeSpan period);
		bool Started(int gameId);
		bool Finished(int gameId);
		int[] StartedGames { get; }
		Game[] ShouldStart { get; }
		int[] FinishedGames { get; }
		int? RegistrableGame { get; }
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
		static Expression<Func<Game, bool>> TimeElapsed(Game.State state, DateTime deadline) => g => g.Current == state && g.LastChange >= deadline;
		static Expression<Func<Game, bool>> InState(Game.State state) => g => g.Current == state;
		static Expression<Func<Game, bool>> InState(Game.State a, Game.State b) => g => g.Current == a || g.Current == b;
		public void RemoveOld(TimeSpan period) => ctx.Games.RemoveAt(TimeElapsed(Game.State.Finished, DateTime.UtcNow - period));
		public Game Find(int gameId) => ctx.Games.Find(gameId);
		public void Finish(int gameId) => ctx.Games.UpdateAt(gameId, g => g.Finish());
		public bool Started(int gameId) => ctx.Games.Find(gameId).Started;
		public bool Finished(int gameId) => ctx.Games.Find(gameId).Finished;
		public Game[] ShouldStart => ctx.Games.Where(TimeElapsed(Game.State.Countdown, DateTime.UtcNow - cfg.Settings.CountdownTime)).ToArray();
		public int[] StartedGames => ctx.Games.Where(InState(Game.State.Started)).Select(g => g.Id).ToArray();
		public int[] FinishedGames => ctx.Games.Where(InState(Game.State.Finished)).Select(g => g.Id).ToArray();
		public int? RegistrableGame => ctx.Games.FirstOrDefault(InState(Game.State.Countdown, Game.State.Created))?.Id;
	}
}