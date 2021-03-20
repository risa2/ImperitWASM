using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Provinces(ImmutableArray<Province> Items, Graph Graph) : IReadOnlyList<Province>
	{
		public Provinces With(ImmutableArray<Province> new_items) => this with { Items = new_items };
		public Provinces With(IEnumerable<Province> new_items) => With(new_items.ToImmutableArray());

		public Province this[int key] => Items[key];
		public IEnumerator<Province> GetEnumerator() => (Items as IEnumerable<Province>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public ImmutableArray<Province>.Builder ToBuilder() => Items.ToBuilder();
		public int Count => Items.Length;

		public int NeighborCount(Province p) => Graph.NeighborCount(p.Order);
		public ImmutableArray<int> NeighborIndices(Province p) => Graph[p.Order];
		public IEnumerable<Province> NeighborsOf(Province p) => NeighborIndices(p).Select(vertex => Items[vertex]);

		public bool Passable(Province from, Province to, int distance, Func<Province, Province, int> difficulty)
		{
			return Graph.Passable(from.Order, to.Order, distance, (start, dest) => difficulty(this[start], this[dest]));
		}
		public IEnumerable<Province> ControlledBy(PlayerIdentity ip) => Items.Where(p => p.IsAllyOf(ip));
		public IEnumerable<IEnumerable<Province>> Split(Func<Province, bool> relevant, Func<Province, Province, bool> passable)
		{
			return Graph.Split(i => relevant(Items[i]), (from, to) => passable(Items[from], Items[to])).Select(list => list.Select(i => Items[i]));
		}
		public int IncomeOf(PlayerIdentity ip)
		{
			var divisions = Split(p => p.IsAllyOf(ip), (from, to) => to.IsAllyOf(ip));
			return divisions.Select(area => area.Sum(province => province.Earnings)).DefaultIfEmpty().Max();
		}

		public bool HasAny(PlayerIdentity ip) => ControlledBy(ip).Any();
		public bool HasNeighborRuledBy(Province province, PlayerIdentity ip) => NeighborsOf(province).Any(p => p.Mainland && p.IsAllyOf(ip));
		public IEnumerable<int> Inhabitable => Items.Indices(it => it.Inhabitable);
		public (PlayerIdentity?, int) Winner => Items.GroupBy(province => province.Ruler).Where(g => g.Key is { Human: true }).Select(g => (g.Key, g.Sum(province => province.Score))).OrderBy(p => p.Item2).FirstOrDefault() is ({ Human: true } human, int finals) ? (human, finals) : (null, 0);
	}
}