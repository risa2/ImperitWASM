using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	[Newtonsoft.Json.JsonConverter(typeof(Conversion.NewtonsoftSettings))]
	public sealed record Settings(int CountdownSeconds, Ratio DefaultInstability, int DebtLimit, int DefaultMoney, int FinalLandsCount, Graph Graph, Ratio Interest, Color LandColor, int PlayerCount, Color MountainsColor, int MountainsWidth, ImmutableArray<string> Names, ImmutableArray<Region> Regions, Color SeaColor, ImmutableArray<SoldierType> SoldierTypes)
	{
		public Player CreatePlayer(int gameId, int i, string name, int land, Password password, bool human) => new Player(new PlayerIdentity(name, i, gameId, human), StartMoney(land), true, ImmutableList<IAction>.Empty, this, password, i == 0);
		public Provinces Provinces => new Provinces(Regions.Select(r => new Province(r, null, r.Soldiers, this)).ToImmutableArray(), Graph);

		public TimeSpan CountdownTime => TimeSpan.FromSeconds(CountdownSeconds);
		public ImmutableDictionary<SoldierType, int> GetSoldierTypeIndices() => SoldierTypes.Lookup();

		public int StartMoney(int province) => DefaultMoney - (Regions[province].Income * 4);
		public int CalculateDebt(int amount) => amount + amount * Interest;
		public int Discount(int amount) => (int)((long)amount * int.MaxValue / (int.MaxValue + Interest.ToInt()));
		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);

		public IEnumerable<SoldierType> RecruitableIn(Region where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		public IEnumerable<SoldierType> RecruitableIn(int where) => RecruitableIn(Regions[where]);
		IEnumerable<string> GetNames(Func<string, int, string> obf) => Enumerable.Range(0, PlayerCount).Select(i => obf(Names[i % Names.Length], i / Names.Length));
		public IEnumerable<(int, Player)> CreateRobots(int gameId, int first, IEnumerable<int> starts, Func<string, int, string> obf)
		{
			foreach (var (i, (start, name)) in starts.Zip(GetNames(obf)).Index())
			{
				yield return (start, CreatePlayer(gameId, i + first, name, start, Password.Random, false));
			}
		}

		public sealed override string ToString() => string.Empty;
	}
}