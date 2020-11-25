using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared;
using ImperitWASM.Shared.State;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Services
{
	public interface IContextService
	{
		Task SaveAsync();

		PlayersAndProvinces GetPlayersAndProvinces(int gameId);
		Game Set(int gameId, IEnumerable<Player> players, IEnumerable<Province> provinces);
		Game Add(IEnumerable<Player> players, IEnumerable<Province> provinces);

		List<PlayersPower> GetPlayersPowers(int gameId);
		int CountPlayersPowers(int gameId);
		void Add(int gameId, PlayersPower psp);

		DbSet<Game> Games { get; }
		DbSet<EntityPlayer> Players { get; }
		DbSet<EntityPlayerPower> PlayerPowers { get; }
		DbSet<EntityProvince> Provinces { get; }
		DbSet<EntitySession> Sessions { get; }
	}
	public class ContextService : IContextService
	{
		readonly Context ctx;
		readonly IConfig cfg;
		public DbSet<Game> Games => ctx.Games ?? throw new NullReferenceException();
		public DbSet<EntityPlayer> Players => ctx.EntityPlayers ?? throw new NullReferenceException();
		public DbSet<EntityPlayerPower> PlayerPowers => ctx.EntityPlayerPowers ?? throw new NullReferenceException();
		public DbSet<EntityProvince> Provinces => ctx.EntityProvinces ?? throw new NullReferenceException();
		public DbSet<EntitySession> Sessions => ctx.EntitySessions ?? throw new NullReferenceException();
		public ContextService(Context ctx, IConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
		}
		public Task SaveAsync() => ctx.SaveChangesAsync();

		// Players and Provinces -------------------------------------------------------------------
		public PlayersAndProvinces GetPlayersAndProvinces(int gameId)
		{
			var game = Games.AsNoTracking().Include(g => g.EntityPlayers).ThenInclude(p => p.EntityPlayerActions)
							.Include(g => g.EntityProvinces).ThenInclude(p => p.EntityProvinceActions).ThenInclude(a => a.EntitySoldiers)
							.Include(g => g.EntityProvinces).ThenInclude(p => p.EntityProvinceActions).ThenInclude(a => a.EntityPlayer).ThenInclude(s => s!.EntityPlayerActions)
							.Single(game => game.Id == gameId);
			return new PlayersAndProvinces(game.GetPlayers(cfg.Settings), new Provinces(game.GetProvinces(cfg.Settings), cfg.Settings.Graph));
		}

		Game Insert(Game game, IEnumerable<Player> players, IEnumerable<Province> provinces)
		{
			var map = players.Select((p, i) => (p, i)).ToImmutableDictionary(x => x.p, x => EntityPlayer.From(x.p, x.i));
			game.EntityPlayers = map.Values.ToList();
			game.EntityProvinces = provinces.Select((province, i) => EntityProvince.From(province, map, cfg.Settings.SoldierTypeIndices, i)).ToList();
			return game;
		}
		public Game Set(int gameId, IEnumerable<Player> players, IEnumerable<Province> provinces)
		{
			Provinces.RemoveRange(Provinces.Where(p => p.GameId == gameId));
			Players.RemoveRange(Players.Where(p => p.GameId == gameId));
			return Insert(Games.Single(game => game.Id == gameId), players, provinces);
		}
		public Game Add(IEnumerable<Player> players, IEnumerable<Province> provinces)
		{
			return Insert(Games.Add(Game.Create).Entity, players, provinces);
		}
		// PlayerPower -------------------------------------------------------------------
		public List<PlayersPower> GetPlayersPowers(int gameId)
		{
			return PlayerPowers.Where(pp => pp.GameId == gameId).AsNoTracking().AsEnumerable().GroupBy(pp => pp.TurnIndex).OrderBy(ppg => ppg.Key).Select(EntityPlayerPower.ConvertOne).ToList();
		}
		public int CountPlayersPowers(int gameId) => PlayerPowers.Where(pp => pp.GameId == gameId).Select(pp => pp.TurnIndex).Distinct().Count();
		public void Add(int gameId, PlayersPower psp)
		{
			int turn = CountPlayersPowers(gameId);
			var game = Games.Include(g => g.EntityPlayerPowers).Single(game => game.Id == gameId);
			psp.Each((pp, i) => game.EntityPlayerPowers!.Add(EntityPlayerPower.From(pp, turn, i)));
		}
	}
}
