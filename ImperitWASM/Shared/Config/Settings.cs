using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Config
{
	public sealed record Settings(int CountdownSeconds, Ratio DefaultInstability, int DebtLimit, int DefaultMoney, int FinalLandsCount, Ratio Interest, Color LandColor, int PlayerCount, Color MountainsColor, int MountainsWidth, ImmutableArray<string> Names, ImmutableArray<ProvinceData> Provinces, Color SeaColor, ImmutableArray<SoldierType> SoldierTypes)
	{
		public Savage Savage => new Savage(this);
		public TimeSpan CountdownTime => TimeSpan.FromSeconds(CountdownSeconds);
		public ImmutableDictionary<SoldierType, int> GetSoldierTypeIndices() => SoldierTypes.Lookup();

		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public int StartMoney(int province) => DefaultMoney - (Provinces[province].Earnings!.Value * 2);
		public static Color ColorOf(int i) => Color.Generate(i, 120.0, 1.0, 1.0);

		public Human CreateHuman(int i, string name, int land, Password password) => Human.Create(ColorOf(i - 1), name, StartMoney(land), this, password);
		public Robot CreateRobot(int i, string name, int land) => Robot.Create(ColorOf(i - 1), name, StartMoney(land), this);
		public Provinces GetProvinces() => new Provinces(Provinces.Select(p => p.Build(this, Savage)).ToImmutableArray(), this);
		public PlayersAndProvinces GetPlayersAndProvinces() => new PlayersAndProvinces(ImmutableArray.Create<Player>(Savage), GetProvinces());

		public int CalculateDebt(int amount) => amount + amount * Interest;
		public int Discount(int amount) => (int)((long)amount * int.MaxValue / (int.MaxValue + Interest.ToInt()));

		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		string GetName(int i, Func<string, int, string> obf) => obf(Names[i % Names.Length], i / Names.Length);
		public IEnumerable<string> GetNames(Func<string, int, string> obf) => Enumerable.Range(0, PlayerCount).Select(i => GetName(i, obf));

		public ImmutableArray<int> NeighborsOf(int vertex) => Provinces[vertex].Neighbors;
		public int NeighborCount(int vertex) => NeighborsOf(vertex).Length;
		public bool Passable(int from, int to, int limit, Func<int, int, int> difficulty)
		{
			var stack = new List<(int Pos, int Distance)>() { (from, 0) };
			bool[] visited = new bool[Provinces.Length];
			visited[from] = true;
			for (int i = 0; i < stack.Count; ++i)
			{
				if (stack[i].Pos == to)
				{
					return true;
				}
				for (int j = 0, len = Provinces[stack[i].Pos].Neighbors.Length; j < len; j++)
				{
					int vertex = Provinces[stack[i].Pos].Neighbors[j];
					if (!visited[vertex] && stack[i].Distance + difficulty(stack[i].Pos, vertex) <= limit)
					{
						stack.Add((vertex, stack[i].Distance + difficulty(stack[i].Pos, vertex)));
						visited[vertex] = true;
					}
				}
			}
			return false;
		}
		public sealed override string ToString() => string.Empty;
	}
}