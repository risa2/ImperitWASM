using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
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
		EntityPlayer Add(Game game, Player player);
		void Add(Game game, Player player, int start);
		bool Do(int gameId, ICommand cmd);
		int FirstActive(int gameId);
		PlayersAndProvinces Next(int gameId);
		PlayersAndProvinces this[int gameId] { get; set; }
		Game Add(PlayersAndProvinces pap);
		Player Player(int gameId, int i);
		Player Active(int gameId);
	}
	public class PlayersProvinces : IPlayersProvinces
	{
		readonly IContextService ctx;
		readonly IConfig cfg;
		readonly IActive active;
		public PlayersProvinces(IContextService ctx, IConfig cfg, IActive active)
		{
			this.ctx = ctx;
			this.cfg = cfg;
			this.active = active;
		}
		public void Add(Game game, Player player, int start)
		{
			ctx.Provinces.Single(p => p.GameId == game.Id && p.Index == start).EntityPlayer = Add(game, player);
		}
		public EntityPlayer Add(Game game, Player player)
		{
			var ePlayer = EntityPlayer.From(player, PlayersCount(game.Id));
			game.EntityPlayers!.Add(ePlayer);
			return ePlayer;
		}
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
			active[gameId] = pap.Players.Select((p, i) => (p, i)).First(p => p.p.Alive && !(p.p is Savage)).i;
			return pap;
		}
		public int FirstActive(int gameId) => ctx.Players.Where(p => p.GameId == gameId && p.Alive && p.Type != EntityPlayer.Kind.Savage).Select(p => p.Index).Min(i => i);
		public Player Player(int gameId, int i) => ctx.Players.Include(p => p.EntityPlayerActions).Single(p => p.Index == i && p.GameId == gameId).Convert(cfg.Settings);
		public Player Active(int gameId) => Player(gameId, ctx.Games.Find(gameId).Active);

		public PlayersAndProvinces this[int gameId]
		{
			get => ctx.GetPlayersAndProvinces(gameId);
			set => ctx.Set(gameId, value.Players, value.Provinces);
		}
		public Game Add(PlayersAndProvinces pap) => ctx.Add(pap.Players, pap.Provinces);
		public int PlayersCount(int gameId) => ctx.Players.Count(p => p.GameId == gameId);
	}
}