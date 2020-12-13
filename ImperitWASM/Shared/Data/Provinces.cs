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
		public IEnumerable<int> NeighborIndices(Province p) => Settings.NeighborsOf(Lookup[p]);
		public IEnumerable<Province> NeighborsOf(Province p) => NeighborIndices(p).Select(vertex => Items[vertex]);
		public IEnumerable<Province> ControlledBy(Player player) => Items.Where(p => p.IsAllyOf(player));

		public Province this[int key] => Items[key];
		public IEnumerator<Province> GetEnumerator() => (Items as IEnumerable<Province>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ImmutableArray<Province>.Builder ToBuilder() => Items.ToBuilder();
		public int Count => Items.Length;
	}
}