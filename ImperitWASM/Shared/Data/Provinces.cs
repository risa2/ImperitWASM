using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Provinces(ImmutableArray<Province> Items, Settings Settings, ImmutableDictionary<Province, int> Lookup) : IReadOnlyList<Province>
	{
		public Provinces(ImmutableArray<Province> Items, Settings Settings) : this(Items, Settings, Items.Lookup()) { }
		public Provinces With(ImmutableArray<Province> new_Items) => new Provinces(new_Items, Settings, Lookup);
		public bool Passable(Province from, Province to, int distance, Func<Province, Province, int> difficulty)
		{
			return Settings.Passable(Lookup[from], Lookup[to], distance, (start, dest) => difficulty(this[start], this[dest]));
		}
		public int NeighborCount(Province p) => Settings.NeighborCount(Lookup[p]);
		public ImmutableArray<int> NeighborIndices(Province p) => Settings.NeighborsOf(Lookup[p]);
		public IEnumerable<Province> NeighborsOf(Province p) => NeighborIndices(p).Select(vertex => Items[vertex]);

		public Province this[int key] => Items[key];
		public IEnumerator<Province> GetEnumerator() => (Items as IEnumerable<Province>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ImmutableArray<Province>.Builder ToBuilder() => Items.ToBuilder();
		public int Count => Items.Length;

		public List<IEnumerable<Province>> Split(Func<Province, bool> relevant, Func<Province, Province, bool> passable)
		{
			bool[] visited = new bool[Count];
			var result = new List<IEnumerable<Province>>();
			for (int from = 0; from < Count; ++from)
			{
				if (!visited[from] && relevant(this[from]))
				{
					visited[from] = true;
					var points = new List<int> { from };
					for (int i = 0; i < points.Count; ++i)
					{
						var neighbors = Settings.NeighborsOf(points[i]);
						for (int n = 0; n < neighbors.Length; ++n)
						{
							if (!visited[neighbors[n]] && passable(this[points[i]], this[neighbors[n]]))
							{
								visited[neighbors[n]] = true;
								points.Add(neighbors[n]);
							}
						}
					}
					result.Add(points.Select(i => this[i]));
				}
			}
			return result;
		}
		public IEnumerable<Province> ControlledBy(Player player) => Items.Where(p => p.IsAllyOf(player));
	}
}