using System;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	[JsonConverter(typeof(Cvt.SoldierTypeConverter))]
	public abstract class SoldierType : IEquatable<SoldierType>, IComparable<SoldierType>
	{
		private int _id;
		public string Name { get; }
		public string Text { get; }
		public string Symbol { get; }
		public int AttackPower { get; }
		public int DefensePower { get; }
		public int Weight { get; }
		public int Price { get; }
		protected SoldierType(string name, string text, string symbol, int attackPower, int defensePower, int weight, int price)
		{
			Name = name;
			Text = text;
			Symbol = symbol;
			AttackPower = attackPower;
			DefensePower = defensePower;
			Weight = weight;
			Price = price;
		}
		public Description Description => new Description(Name, Text, Symbol);
		public int Power => AttackPower + DefensePower;
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
