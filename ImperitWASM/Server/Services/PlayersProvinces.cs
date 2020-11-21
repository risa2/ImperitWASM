using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.Motion.Commands;
using ImperitWASM.Shared.State;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Services
{
	public interface IPlayersProvinces
	{
		int PlayersCount(int gameId);
		void Add(int gameId, Player player);
		void Add(int gameId, Player player, Soldiers soldiers, int start);
		bool Do(int gameId, ICommand cmd);
		void ResetActive(int gameId);
		ImmutableArray<Player> Players(int gameId);
		PlayersAndProvinces Next(int gameId);
		PlayersAndProvinces this[int gameId] { get; set; }
		void AddPaP(int gameId, PlayersAndProvinces pap);
		Player Player(int gameId, int i);
		Player Active(int gameId);
	}
	public class PlayersProvinces : IPlayersProvinces
	{
		private readonly IContextService ctx;
		private readonly IConfig cfg;
		private readonly IActive active;
		public PlayersProvinces(IContextService ctx, IConfig cfg, IActive active)
		{
			this.ctx = ctx;
			this.cfg = cfg;
			this.active = active;
		}
		public void Add(int gameId, Player player, Soldiers soldiers, int start)
		{
			Add(gameId, player);
			var province = ctx.Provinces.Single(p => p.GameId == gameId && p.Index == start);
			province.EntitySoldier = EntitySoldier.From(soldiers, cfg.Settings.SoldierTypeIndices);
		}
		public void Add(int gameId, Player player) => ctx.Players.Add(EntityPlayer.From(player, PlayersCount(gameId), gameId));
		public bool Do(int gameId, ICommand cmd)
		{
			var (new_pap, success) = this[gameId].Do(cmd);
			this[gameId] = new_pap;
			return success;
		}
		public PlayersAndProvinces Next(int gameId)
		{
			int i = active[gameId];
			var pap = this[gameId] = this[gameId].Do(new NextTurn()).Item1.Act(i);
			active[gameId] = GetNextActive(gameId, i);
			return pap;
		}
		public void ResetActive(int gameId) => active[gameId] = GetNextActive(gameId, 0);
		public Player Player(int gameId, int i) => ctx.Players.Include(p => p.EntityPlayerActions).Single(p => p.Index == i && p.GameId == gameId).Convert(cfg.Settings);
		public Player Active(int gameId) => Player(gameId, ctx.Games.Find(gameId).Active);

		private int GetNextActive(int gameId, int active)
		{
			return ctx.Players.Where(p => p.GameId == gameId && p.Alive && p.Type == EntityPlayer.Kind.Human).Select(p => p.Index).OrderBy(i => i).ToArray().FirstIfOrFirst(i => i > active);
		}
		public PlayersAndProvinces this[int gameId]
		{
			get
			{
				var players = ctx.GetPlayers(gameId);
				var provinces = ctx.GetProvinces(gameId, players);
				return new PlayersAndProvinces(players, new Provinces(provinces, cfg.Settings.Graph));
			}
			set => ctx.SetPlayers(gameId, value.Players).SetProvinces(gameId, value.Provinces);
		}
		public void AddPaP(int gameId, PlayersAndProvinces pap) => ctx.AddPlayersAndProvinces(gameId, pap.Players, pap.Provinces);
		public int PlayersCount(int gameId) => ctx.Players.Count(p => p.GameId == gameId);
		public ImmutableArray<Player> Players(int gameId) => ctx.GetPlayers(gameId);
	}
}