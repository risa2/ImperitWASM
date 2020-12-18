using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Config;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Services
{
	public interface IGameService
	{
		Game Find(int gameId);
		Game? FindNoTracking(int gameId);
		void RemoveOld(TimeSpan period);
		ImmutableArray<Game> ShouldStart { get; }
		ImmutableArray<int> FinishedGames { get; }
		int? RegistrableGame { get; }
	}
	public class GameService : IGameService
	{
		readonly IContextService ctx;
		readonly Settings settings;
		public GameService(IContextService ctx, Settings settings)
		{
			this.ctx = ctx;
			this.settings = settings;
		}

		static Expression<Func<Game, bool>> TimeElapsed(Game.State state, DateTime time) => g => g.Current == state && g.LastChange <= time;
		static Expression<Func<Game, bool>> InState(Game.State state) => g => g.Current == state;
		static Expression<Func<Game, bool>> InState(Game.State a, Game.State b) => g => g.Current == a || g.Current == b;
		public void RemoveOld(TimeSpan period) => ctx.Games.RemoveAt(TimeElapsed(Game.State.Finished, DateTime.UtcNow - period));
		public Game Find(int gameId) => ctx.Games.Find(gameId);
		public Game? FindNoTracking(int gameId) => ctx.Games.AsNoTracking().SingleOrDefault(game => game.Id == gameId);
		public ImmutableArray<Game> ShouldStart => ctx.Games.Where(TimeElapsed(Game.State.Countdown, DateTime.UtcNow - settings.CountdownTime)).ToImmutableArray();
		public ImmutableArray<int> FinishedGames => ctx.Games.Where(InState(Game.State.Finished)).Select(g => g.Id).ToImmutableArray();
		public int? RegistrableGame => ctx.Games.FirstOrDefault(InState(Game.State.Countdown, Game.State.Created))?.Id;
	}
}