using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.SoldierTypeConverter))]
	public abstract record SoldierType(Description Description, int AttackPower, int DefensePower, int Weight, int Price) : IEquatable<SoldierType>, IComparable<SoldierType>
	{
		public string Name => Description.Name;
		public string Symbol => Description.Symbol;
		public ImmutableArray<string> Text => Description.Text;
		public int Power => AttackPower + DefensePower;
		public abstract bool IsRecruitable(Province province);
		public abstract int CanSustain(Province province);
		public abstract int CanMove(PlayersAndProvinces pap, Province from, Province to);
		public bool CanMoveAlone(PlayersAndProvinces pap, Province from, Province to) => CanMove(pap, from, to) >= Weight;
		public int CompareTo(SoldierType? type) => Symbol.CompareTo(type?.Symbol);
		public override int GetHashCode() => Symbol.GetHashCode();
		public virtual bool Equals(SoldierType? t) => CompareTo(t) == 0;
	}
}
