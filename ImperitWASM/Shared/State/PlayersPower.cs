using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Conversion.PlayersPowerConverter))]
	public class PlayersPower : IReadOnlyList<PlayerPower>, Conversion.IEntity<PlayersPower,bool>
	{
		readonly ImmutableArray<PlayerPower> pp;
		public PlayersPower(ImmutableArray<PlayerPower> pp) => this.pp = pp;
		public PlayerPower this[int index] => pp[index];
		public int Count => pp.Length;
		public IEnumerator<PlayerPower> GetEnumerator() => ((IEnumerable<PlayerPower>)pp).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public ImmutableArray<double> GetRatios()
		{
			var sum = pp.Sum(p => p.Soldiers + p.Money);
			return pp.Select(p => (double)(p.Soldiers + p.Money) / sum).ToImmutableArray();
		}
		public int TotalSum => pp.Sum(p => p.Total);
		public int TotalAvg => TotalSum / Count;
		public int TotalMax => pp.Max(pp => pp.Total);
		static (int Soldiers, int Income, int Lands, int Finals) SoldiersIncome(IEnumerable<Province> provinces)
		{
			return provinces.Aggregate((0, 0, 0, 0), (pair, prov) => (pair.Item1 + prov.Soldiers.Power, pair.Item2 + prov.Earnings, pair.Item3 + 1, pair.Item4 + (prov is Land l && l.IsFinal ? 1 : 0)));
		}
		public static PlayersPower Compute(PlayersAndProvinces pap)
		{
			return new PlayersPower(pap.Compute(p => (p.Money, Alive: p.Alive && p is Human), ps => SoldiersIncome(ps), (x, y) => x.Alive ? new PlayerPower(true, y.Income, y.Lands, x.Money, y.Soldiers, y.Finals) : new PlayerPower(false, 0, 0, 0, 0, 0)));
		}
		public PlayersPower Convert(int _, bool __) => this;
	}
}