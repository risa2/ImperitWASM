﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using ImperitWASM.Server.Load;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Services
{
	public interface IGameService
	{
		Game Find(int gameId);
		Game FindNoTracking(int gameId);
		void Finish(int gameId);
		void RemoveOld(TimeSpan period);
		bool Started(int gameId);
		List<Game> ShouldStart { get; }
		ImmutableArray<int> StartedGames { get; }
		ImmutableArray<int> FinishedGames { get; }
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

		static Expression<Func<Game, bool>> TimeElapsed(Game.State state, DateTime time) => g => g.Current == state && g.LastChange <= time;
		static Expression<Func<Game, bool>> InState(Game.State state) => g => g.Current == state;
		static Expression<Func<Game, bool>> InState(Game.State a, Game.State b) => g => g.Current == a || g.Current == b;
		public void RemoveOld(TimeSpan period) => ctx.Games.RemoveAt(TimeElapsed(Game.State.Finished, DateTime.UtcNow - period));
		public Game Find(int gameId) => ctx.Games.Find(gameId);
		public Game FindNoTracking(int gameId) => ctx.Games.AsNoTracking().Single(game => game.Id == gameId);
		public void Finish(int gameId) => ctx.Games.UpdateAt(gameId, g => g.Finish());
		public bool Started(int gameId) => ctx.Games.Find(gameId).Started;
		public List<Game> ShouldStart => ctx.Games.Include(g => g.EntityPlayers).Where(TimeElapsed(Game.State.Countdown, DateTime.UtcNow - cfg.Settings.CountdownTime)).ToList();
		public ImmutableArray<int> StartedGames => ctx.Games.Where(InState(Game.State.Started)).Select(g => g.Id).ToImmutableArray();
		public ImmutableArray<int> FinishedGames => ctx.Games.Where(InState(Game.State.Finished)).Select(g => g.Id).ToImmutableArray();
		public int? RegistrableGame => ctx.Games.FirstOrDefault(InState(Game.State.Countdown, Game.State.Created))?.Id;
	}
}