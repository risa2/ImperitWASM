using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public class Provinces : IReadOnlyList<Province>
	{
		readonly ImmutableArray<Province> provinces;
		readonly Graph graph;
		readonly ImmutableDictionary<Province, int> lookup;
		public Provinces(ImmutableArray<Province> provinces, Graph graph, ImmutableDictionary<Province, int>? lookup = null)
		{
			this.provinces = provinces;
			this.graph = graph;
			this.lookup = lookup ?? provinces.Lookup();
		}
		public Provinces With(ImmutableArray<Province> new_provinces) => new Provinces(new_provinces, graph, lookup);
		public bool Passable(Province from, Province to, int distance, Func<Province, Province, int> difficulty)
		{
			return graph.Passable(lookup[from], lookup[to], distance, (start, dest) => difficulty(this[start], this[dest]));
		}
		public int NeighborCount(Province p) => graph.NeighborCount(lookup[p]);
		public IEnumerable<Province> NeighborsOf(Province p) => graph[lookup[p]].Select(vertex => provinces[vertex]);
		public IEnumerable<Province> ControlledBy(Player player) => provinces.Where(p => p.IsAllyOf(player));

		public Province this[int key] => provinces[key];
		public IEnumerator<Province> GetEnumerator() => (provinces as IEnumerable<Province>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ImmutableArray<Province>.Builder ToBuilder() => provinces.ToBuilder();
		public int Count => provinces.Length;
	}
}