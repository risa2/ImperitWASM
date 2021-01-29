using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IProvinceLoader
	{
		int PlayersCount(int gameId);
		void Add(int gameId, Player player, int order);
		Task<bool> AddAsync(int gameId, ICommand cmd);
		Provinces this[int gameId] { get; set; }
		Game Add(Provinces pap);
		Player Player(int gameId, int i);
		Province? Province(int gameId, int i);
		bool GameExists(int gameId) => PlayersCount(gameId) > 0;
		bool IsNameFree(string name);
		string ObsfuscateName(string name, int repetition);
	}
	public class ProvinceLoader : IProvinceLoader
	{
		readonly IDatabase db;
		readonly Settings settings;
		public ProvinceLoader(IDatabase db, Settings settings)
		{
			this.db = db;
			this.settings = settings;
		}
		public void Add(int gameId, Player player, int i, int start)
		{
			db.Provinces.Include(p => p.EntityProvinceActions).Single(p => p.GameId == gameId && p.Index == start).EntityProvinceActions!.Single(a => a.Type == EntityProvinceAction.Kind.Existence).EntityPlayer = Add(gameId, player, i);
		}
		public EntityPlayer Add(int gameId, Player player, int i)
		{
			var ePlayer = EntityPlayer.From(player, i);
			ePlayer.GameId = gameId;
			return db.Players.Add(ePlayer).Entity;
		}
		public async Task<bool> AddAsync(int gameId, ICommand cmd)
		{
			var (new_pap, success) = this[gameId].TryAdd(cmd);
			this[gameId] = new_pap;
			await db.SaveAsync();
			return success;
		}
		public Player Player(int gameId, int i) => db.Players.Include(p => p.EntityPlayerActions).AsNoTracking().SingleOrDefault(p => p.Index == i && p.GameId == gameId)?.Convert(settings) ?? settings.Savage;
		public Province? Province(int gameId, int i) => db.Provinces.AsNoTracking().Include(p => p.EntityProvinceActions).ThenInclude(s => s.EntitySoldiers)
							.Include(p => p.EntityProvinceActions).ThenInclude(a => a.EntityPlayer).ThenInclude(s => s!.EntityPlayerActions)
							.SingleOrDefault(p => p.GameId == gameId && p.Index == i)?.Convert(settings);
		public PlayersAndProvinces this[int gameId]
		{
			get => db.GetPlayersAndProvinces(gameId);
			set => db.Set(gameId, value.Players, value.Provinces);
		}
		public Game Add(PlayersAndProvinces pap) => db.Add(pap.Players, pap.Provinces);
		public int PlayersCount(int gameId) => db.Players.Count(p => p.GameId == gameId);

		public bool IsNameFree(string name) => name.All(c => !char.IsDigit(c)) && db.Players.All(p => p.Name != name);
		public string ObsfuscateName(string original, int repetition)
		{
			original = original.Trim();
			return db.Players.Select(p => p.Name).Where(name => name.StartsWith(original)).ToList().Select(name => name[original.Length..]).Where(suf => suf.All(c => c is >= '0' and <= '9')).DefaultIfEmpty("").Max(n => n ?? "") switch
			{
				{ Length: > 0 } suf when suf[^1] >= '0' && suf[^1] < (char)('9' - repetition) => original + suf[..^1] + (char)(suf[^1] + 1 + repetition),
				var suf => original + suf + (repetition + 1)
			};
		}
	}
}