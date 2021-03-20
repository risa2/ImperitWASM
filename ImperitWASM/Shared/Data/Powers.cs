using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.Data
{
	public record Powers(ImmutableArray<Power> Items)
	{
		public record PlayerFinals(int Player, int Count);
		public Power this[int i] => Items[i];
		public int Length => Items.Length;
		public ImmutableArray<double> GetRatios()
		{
			int sum = Items.Sum(p => p.Soldiers + p.Money);
			return Items.Select(p => (double)(p.Soldiers + p.Money) / sum).ToImmutableArray();
		}
		public int TotalSum => Items.Sum(p => p.Total);
		public int TotalMax => Items.Max(p => p.Total);
		public int TotalAvg => TotalSum / Length;
		public ImmutableArray<double> ChangesFrom(Powers previous) => Items.Zip(previous.Items, (next, prev) => (double)next.Total / prev.Total - 1.0).ToImmutableArray();
		public IEnumerable<PlayerFinals> Finals => Items.Select((p, i) => new PlayerFinals(i, p.Final)).Where(r => r.Count > 0);
		public int MaxFinalsPlayer => Items.Select((p, i) => (p, i)).OrderByDescending(x => x.p.Final).First().i;
	}
}