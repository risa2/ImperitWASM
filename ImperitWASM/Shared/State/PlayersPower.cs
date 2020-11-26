using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public record PlayersPower(ImmutableArray<PlayerPower> Powers) : IReadOnlyList<PlayerPower>
	{
		public PlayerPower this[int i] => Powers[i];
		public int Count => Powers.Length;
		public IEnumerator<PlayerPower> GetEnumerator() => (Powers as IEnumerable<PlayerPower>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public ImmutableArray<double> GetRatios()
		{
			int sum = Powers.Sum(p => p.Soldiers + p.Money);
			return Powers.Select(p => (double)(p.Soldiers + p.Money) / sum).ToImmutableArray();
		}
		public int TotalSum => Powers.Sum(p => p.Total);
		public int TotalAvg => TotalSum / Count;
		public int TotalMax => Powers.Max(pp => pp.Total);
	}
}