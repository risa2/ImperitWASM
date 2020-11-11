using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.PlayersPowerConverter))]
	public class PlayersPower : IReadOnlyList<PlayerPower>
	{
		readonly ImmutableArray<PlayerPower> arr;
		public PlayersPower(ImmutableArray<PlayerPower> pp) => arr = pp;
		public PlayerPower this[int i] => arr[i];
		public int Count => arr.Length;
		public IEnumerator<PlayerPower> GetEnumerator() => ((IEnumerable<PlayerPower>)arr).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public ImmutableArray<double> GetRatios()
		{
			var sum = arr.Sum(p => p.Soldiers + p.Money);
			return arr.Select(p => (double)(p.Soldiers + p.Money) / sum).ToImmutableArray();
		}
		public int TotalSum => arr.Sum(p => p.Total);
		public int TotalAvg => TotalSum / Count;
		public int TotalMax => arr.Max(pp => pp.Total);
		static PlayerPower ComputeOne(Player p, IEnumerable<Province> provinces)
		{
			return new PlayerPower(p.Alive, provinces.OfType<Land>().Sum(p => p.Earnings), provinces.Count(), p.Money, provinces.Sum(p => p.Soldiers.Power), provinces.Count(p => p is Land));
		}
		public static PlayersPower Compute(PlayersAndProvinces pap)
		{
			return new PlayersPower(pap.PlayersProvinces.Where(pp => pp.Player is Human).Select(pp => ComputeOne(pp.Player, pp.Provinces)).ToImmutableArray());
		}
	}
}