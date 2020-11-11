using System;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.SoldierTypeConverter))]
	public abstract class SoldierType : IEquatable<SoldierType>, IComparable<SoldierType>
	{
		public abstract Description Description { get; }
		public string Name => Description.Name;
		public string Symbol => Description.Symbol;
		public string Text => Description.Text;
		public abstract int AttackPower { get; }
		public abstract int DefensePower { get; }
		public int Power => AttackPower + DefensePower;
		public abstract int Weight { get; }
		public abstract int Price { get; }
		public abstract bool IsRecruitable(Province province);
		public abstract int CanSustain(Province province);
		public abstract int CanMove(PlayersAndProvinces pap, Province from, Province to);
		public bool CanMoveAlone(PlayersAndProvinces pap, Province from, Province to) => CanMove(pap, from, to) >= Weight;
		public int CompareTo(SoldierType? type) => Symbol.CompareTo(type?.Symbol);
		public sealed override int GetHashCode() => Symbol.GetHashCode();
		public virtual bool Equals(SoldierType? t) => CompareTo(t) == 0;
		public sealed override bool Equals(object? obj) => Equals(obj as SoldierType);
	}
}
