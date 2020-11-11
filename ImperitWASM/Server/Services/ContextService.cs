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

		ImmutableArray<Player> GetPlayers(int gameId);
		IContextService SetPlayers(int gameId, IEnumerable<Player> players);
		IContextService AddPlayers(int gameId, IEnumerable<Player> players);

		ImmutableArray<Province> GetProvinces(int gameId, IReadOnlyList<Player> players);
		IContextService SetProvinces(int gameId, IEnumerable<Province> provinces);
		IContextService AddProvinces(int gameId, IEnumerable<Province> provinces);

		List<PlayersPower> GetPlayersPowers(int gameId);
		IContextService AddPlayersPower(int gameId, PlayersPower psp);
		int CountPlayersPowers(int gameId);

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
		public DbSet<Game> Games => ctx.EntityGames ?? throw new NullReferenceException();
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
		//Players--------------------------------------------------------------
		IQueryable<EntityPlayer> GetEPlayers(int gameId) => Players.Where(p => p.GameId == gameId).OrderBy(p => p.Index).Include(p => p.EntityPlayerActions);
		public ImmutableArray<Player> GetPlayers(int gameId) => GetEPlayers(gameId).Select(p => p.Convert(cfg.Settings)).ToImmutableArray();
		public IContextService SetPlayers(int gameId, IEnumerable<Player> players)
		{
			GetEPlayers(gameId).ToArray().Zip(players).Each(it => it.First.Assign(it.Second));
			return this;
		}
		public IContextService AddPlayers(int gameId, IEnumerable<Player> players)
		{
			players.Each((player, i) => Players.Add(EntityPlayer.From(player, i, gameId)));
			return this;
		}
		//Provinces------------------------------------------------------------
		IQueryable<EntityProvince> GetEProvinces(int gameId) => Provinces.Where(p => p.GameId == gameId).OrderBy(p => p.Index)
						.Include(p => p.EntitySoldier).ThenInclude(s => s.EntitySoldierPairs)
						.Include(p => p.EntityProvinceActions).ThenInclude(a => a.EntitySoldier).ThenInclude(s => s.EntitySoldierPairs);
		public ImmutableArray<Province> GetProvinces(int gameId, IReadOnlyList<Player> players)
		{
			Province Convert(EntityProvince p) => p.Convert(cfg.Settings, players);
			return GetEProvinces(gameId).Select(Convert).ToImmutableArray();
		}
		public IContextService SetProvinces(int gameId, IEnumerable<Province> provinces)
		{
			var map = GetEPlayers(gameId).ToImmutableDictionary(p => p.Convert(cfg.Settings), p => p.Index);
			GetEProvinces(gameId).ToArray().Zip(provinces).Each(it => it.First.Assign(it.Second, map, cfg.Settings.SoldierTypeIndices));
			return this;
		}
		public IContextService AddProvinces(int gameId, IEnumerable<Province> provinces)
		{
			var map = GetEPlayers(gameId).ToImmutableDictionary(p => p.Convert(cfg.Settings), p => p.Index);
			provinces.Each((province, i) => Provinces.Add(EntityProvince.From(province, map, cfg.Settings.SoldierTypeIndices, gameId, i)));
			return this;
		}
		//PlayerPower----------------------------------------------------------
		public List<PlayersPower> GetPlayersPowers(int gameId)
		{
			return PlayerPowers.Where(pp => pp.GameId == gameId).GroupBy(pp => pp.TurnIndex).OrderBy(ppg => ppg.Key).Select(EntityPlayerPower.ConvertOne).ToList();
		}
		public int CountPlayersPowers(int gameId) => PlayerPowers.Count(pp => pp.GameId == gameId) / Players.Count(pp => pp.GameId == gameId);
		public IContextService AddPlayersPower(int gameId, PlayersPower psp)
		{
			int turn = CountPlayersPowers(gameId);
			PlayerPowers.AddRange(psp.Select((pp, i) => EntityPlayerPower.From(pp, gameId, turn, i)));
			return this;
		}
	}
}
