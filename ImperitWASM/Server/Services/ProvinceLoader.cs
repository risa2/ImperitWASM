using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface IProvinceLoader
	{
		Provinces this[int gameId] { get; }
		Province this[int gameId, int order] { get; }
		void Set(int gameId, IEnumerable<Province> provinces, bool fromTransaction);
		void Set(int gameId, Province province, bool fromTransaction);
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

		IEnumerable<Province> Get(string where, int gameId, object? arg = null) => db.Query<int, int?, string?, bool?, int?, int?>(@"
			SELECT Province.""Order"", Player.""Order"", Player.Name, Player.Human, ProvinceRegiment.Type, ProvinceRegiment.""Count"" FROM Province
			LEFT JOIN ProvinceRegiment ON ProvinceRegiment.GameId=Province.GameId AND ProvinceRegiment.Province=Province.""Order""
			LEFT JOIN Player ON Player.GameId=Province.GameId AND Player.""Order""=Province.Player
			WHERE Province.GameId=@0 " + where + @" ORDER BY Province.""Order""", gameId, arg).GroupBy(p => p.Item1).Select(p_group =>
		{
			var (province, order, name, human, _, _) = p_group.First();
			var regiments = p_group.Where(r => r.Item5 is not null && r.Item6 is not null).Select(r => new Regiment(settings.SoldierTypes[r.Item5!.Value], r.Item6!.Value));
			return new Province(settings.Regions[province], order is int p ? new PlayerIdentity(name!, p, gameId, human!.Value) : null, new Soldiers(regiments), settings);
		});
		public Provinces this[int gameId] => new Provinces(Get("", gameId).ToImmutableArray(), settings.Graph);
		public Province this[int gameId, int order] => Get("AND Province.\"Order\"=@1", gameId, order).First();

		void Insert(int gameId, Province province)
		{
			db.Command(@"INSERT INTO Province (GameId, ""Order"", Player) VALUES (@0,@1,@2)", gameId, province.Order, province.Ruler?.Order);
			foreach (var (type, count) in province.Soldiers)
			{
				db.Command(@"INSERT INTO ProvinceRegiment (""Count"", Type, GameId, Province) VALUES (@0,@1,@2,@3)", count, type_indices[type], gameId, province.Order);
			}
		}
		public void Set(int gameId, IEnumerable<Province> provinces, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			db.Command("DELETE FROM Province WHERE GameId=@0", gameId);
			foreach (var province in provinces)
			{
				Insert(gameId, province);
			}
		});
		public void Set(int gameId, Province province, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			db.Command(@"DELETE FROM Province WHERE GameId=@0 AND ""Order""=@1", gameId, province.Order);
			Insert(gameId, province);
		});
	}
}