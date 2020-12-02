using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public record Settings(Ratio DefaultInstability, Ratio Interest, int DebtLimit, int DefaultMoney, int MountainsWidth, Color LandColor, Color MountainsColor, Color SeaColor, ImmutableArray<SoldierType> SoldierTypes, int FinalLandsCount, int CountdownSeconds, ImmutableArray<ProvinceData> Provinces)
	{
		public TimeSpan CountdownTime => TimeSpan.FromSeconds(CountdownSeconds);
		public ImmutableDictionary<SoldierType, int> GetSoldierTypeIndices() => SoldierTypes.Lookup();

		public Ratio Instability(Soldiers now, Soldiers start) => DefaultInstability.Adjust(Math.Max(start.DefensePower - now.DefensePower, 0), start.DefensePower);
		public int StartMoney(int earnings) => DefaultMoney - (earnings * 2);

		public IEnumerable<SoldierType> RecruitableTypes(Province where) => SoldierTypes.Where(t => t.IsRecruitable(where));
		public Provinces GetProvinces() => new Provinces(Provinces.Select(p => p.Build(this, new Savage())).ToImmutableArray(), this);

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
	}
}