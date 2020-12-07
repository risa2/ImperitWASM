using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Shared.State
{
	public record PlayersPower(ImmutableArray<PlayerPower> Powers)
	{
		public record PlayerFinals(int Player, int Count);
		public PlayerPower this[int i] => Powers[i];
		public int Length => Powers.Length;
		public ImmutableArray<double> GetRatios()
		{
			int sum = Powers.Sum(p => p.Soldiers + p.Money);
			return Powers.Select(p => (double)(p.Soldiers + p.Money) / sum).ToImmutableArray();
		}
		public int TotalSum => Powers.Sum(p => p.Total);
		public int TotalMax => Powers.Max(p => p.Total);
		public int TotalAvg => TotalSum / Length;
		public ImmutableArray<double> ChangesFrom(PlayersPower previous) => Powers.Zip(previous.Powers, (next, prev) => (double)next.Total / prev.Total - 1.0).ToImmutableArray();
		public IEnumerable<PlayerFinals> Finals => Powers.Select((p, i) => new PlayerFinals(i, p.Final)).Where(r => r.Count > 0);
	}
}