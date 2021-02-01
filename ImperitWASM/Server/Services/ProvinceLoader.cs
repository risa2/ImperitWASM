using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IProvinceLoader
	{
		Provinces Get(int gameId, IReadOnlyList<Player> players);
		Province Get(int gameId, int order, IReadOnlyList<Player> players);
		void Set(int gameId, IEnumerable<Province> provinces, bool fromTransaction);
		void Set(int gameId, int order, Province province, bool fromTransaction);
	}
	public class ProvinceLoader : IProvinceLoader
	{
		readonly IDatabase db;
		readonly Settings settings;
		readonly ImmutableDictionary<SoldierType, int> type_indices;
		public ProvinceLoader(IDatabase db, Settings settings)
		{
			this.db = db;
			this.settings = settings;
			type_indices = settings.GetSoldierTypeIndices();
		}

		IEnumerable<Province> Get(string where, IReadOnlyList<Player> players, params object[] args) => db.Query<int, int?, int?, int?>(@"
			SELECT Province.""Order"", Player.""Order"", ProvinceRegiment.Type, ProvinceRegiment.Count FROM Province
			LEFT JOIN ProvinceRegiment ON ProvinceRegiment.ProvinceId = Province.Id
			LEFT JOIN Player ON Player.Name = Province.PlayerName
			ORDER BY Province.""Order"" " + where, args).GroupBy(p => p.Item1).Select(p_group =>
		{
			var (province, player, count, type) = p_group.First();
			var regiments = p_group.Where(r => r.Item3 is not null && r.Item4 is not null).Select(r => new Regiment(settings.SoldierTypes[r.Item3!.Value], r.Item4!.Value));
			return new Province(settings.Regions[province], player is int P ? players[P] : null, new Soldiers(regiments), settings);
		});
		public Provinces Get(int gameId, IReadOnlyList<Player> players) => new Provinces(Get("WHERE Province.GameId = @x0", players, gameId).ToImmutableArray(), settings.Graph);
		public Province Get(int gameId, int order, IReadOnlyList<Player> players) => Get("WHERE Province.GameId=@x0 AND Province.Order=@x1", players, gameId, order).First();

		void Insert(int gameId, Province province)
		{
			db.Command("INSERT INTO Province (GameId, Order, PlayerName) VALUES (@x0,@x1,@x2)", gameId, province.Order, province.Player?.Name);
			long id = db.Query<long>("SELECT last_insert_rowid()").First();
			foreach (var (type, count) in province.Soldiers)
			{
				db.Command("INSERT INTO ProvinceRegiment (Count, Type, ProvinceId) VALUES (@x0,@x1,@x2)", count, type_indices[type], id);
			}
		}
		public void Set(int gameId, IEnumerable<Province> provinces, bool fromTransaction)
		{
			db.Transaction(!fromTransaction, () =>
			{
				db.Command("DELETE FROM Province WHERE GameId=@x0", gameId);
				foreach (var province in provinces)
				{
					Insert(gameId, province);
				}
			});
		}
		public void Set(int gameId, int order, Province province, bool fromTransaction)
		{
			db.Transaction(!fromTransaction, () =>
			{
				db.Command("DELETE FROM Province WHERE GameId=@x0 AND Order=@x1", gameId, order);
				Insert(gameId, province);
			});
		}
	}
}