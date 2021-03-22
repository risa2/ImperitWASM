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
		ImmutableArray<double> GetRatios(double sum, System.Func<Power, double> value) => Items.Select(p => value(p) / sum).ToImmutableArray();
		public ImmutableArray<double> Ratios => GetRatios(Items.Sum(p => p.Soldiers + p.Money), p => p.Soldiers + p.Money);
		public int TotalSum => Items.Sum(p => p.Total);
		public int TotalMax => Items.Max(p => p.Total);
		public int TotalAvg => TotalSum / Length;
		public IEnumerable<double> ChangesFrom(Powers previous) => Items.Zip(previous.Items, (next, prev) => prev.Alive ? (double)next.Total / prev.Total - 1.0 : 0.0);
		public IEnumerable<PlayerFinals> Finals => Items.Select((p, i) => new PlayerFinals(i, p.Final)).Where(r => r.Count > 0);
		public int MaxFinalsPlayer => Items.Select((p, i) => (p, i)).OrderByDescending(x => x.p.Final).First().i;
	}
}