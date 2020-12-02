using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Services
{
	public interface IPlayersProvinces
	{
		int PlayersCount(int gameId);
		EntityPlayer Add(int gameId, Player player, int i);
		void Add(int gameId, Player player, int i, int start);
		Task<bool> AddAsync(int gameId, ICommand cmd);
		PlayersAndProvinces this[int gameId] { get; set; }
		Game Add(PlayersAndProvinces pap);
		Player Player(int gameId, int i);
		Province? Province(int gameId, int i);
		bool GameExists(int gameId) => PlayersCount(gameId) > 0;
		bool IsNameFree(string name);
		string ObsfuscateName(string name);
	}
	public class PlayersProvinces : IPlayersProvinces
	{
		readonly IContextService ctx;
		readonly Settings settings;
		public PlayersProvinces(IContextService ctx, Settings settings)
		{
			this.ctx = ctx;
			this.settings = settings;
		}
		public void Add(int gameId, Player player, int i, int start)
		{
			ctx.Provinces.Include(p => p.EntityProvinceActions).Single(p => p.GameId == gameId && p.Index == start).EntityProvinceActions!.Single(a => a.Type == EntityProvinceAction.Kind.Existence).EntityPlayer = Add(gameId, player, i);
		}
		public EntityPlayer Add(int gameId, Player player, int i)
		{
			var ePlayer = EntityPlayer.From(player, i);
			ePlayer.GameId = gameId;
			return ctx.Players.Add(ePlayer).Entity;
		}
		public async Task<bool> AddAsync(int gameId, ICommand cmd)
		{
			var (new_pap, success) = this[gameId].Add(cmd);
			this[gameId] = new_pap;
			await ctx.SaveAsync();
			return success;
		}
		public Player Player(int gameId, int i) => ctx.Players.Include(p => p.EntityPlayerActions).AsNoTracking().SingleOrDefault(p => p.Index == i && p.GameId == gameId)?.Convert(settings) ?? new Savage();
		public Province? Province(int gameId, int i) => ctx.Provinces.AsNoTracking().Include(p => p.EntityProvinceActions).ThenInclude(s => s.EntitySoldiers)
							.Include(p => p.EntityProvinceActions).ThenInclude(a => a.EntityPlayer).ThenInclude(s => s!.EntityPlayerActions)
							.SingleOrDefault(p => p.GameId == gameId && p.Index == i)?.Convert(settings);
		public PlayersAndProvinces this[int gameId]
		{
			get => ctx.GetPlayersAndProvinces(gameId);
			set => ctx.Set(gameId, value.Players, value.Provinces);
		}
		public Game Add(PlayersAndProvinces pap) => ctx.Add(pap.Players, pap.Provinces);
		public int PlayersCount(int gameId) => ctx.Players.Count(p => p.GameId == gameId);

		public bool IsNameFree(string name) => ctx.Players.All(p => p.Name != name);
		public string ObsfuscateName(string original)
		{
			original = original.Trim();
			int number = ctx.Players.Select(p => p.Name).Where(name => name.StartsWith(original)).OrderBy(n => n.Length).DefaultIfEmpty("").ToList().Max(name => int.TryParse(name[original.Length..], out int value) ? value : -1);
			return number < 0 ? original : original + (number + 1);
		}
	}
}