using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public interface IProvinces : IReadOnlyList<Province>
	{
		bool Passable(int from, int to, int distance, Func<Province, Province, int> difficulty);
	}
	public class Provinces : IProvinces
	{
		readonly ImmutableArray<Province> provinces;
		readonly Graph graph;
		public Provinces(ImmutableArray<Province> provinces, Graph graph)
		{
			this.provinces = provinces;
			this.graph = graph;
		}
		public Provinces With(ImmutableArray<Province> new_provinces) => new Provinces(new_provinces, graph);
		public bool Passable(int from, int to, int distance, Func<Province, Province, int> difficulty)
		{
			return graph.Passable(from, to, distance, (start, dest) => difficulty(this[start], this[dest]));
		}
		public int NeighborCount(int id) => graph.NeighborCount(id);
		public IEnumerable<Province> NeighborsOf(int id) => graph[id].Select(vertex => provinces[vertex]);

		public Province this[int key] => provinces[key];
		public IEnumerator<Province> GetEnumerator() => (provinces as IEnumerable<Province>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ImmutableArray<Province>.Builder ToBuilder() => provinces.ToBuilder();
		public int Count => provinces.Length;
	}
}