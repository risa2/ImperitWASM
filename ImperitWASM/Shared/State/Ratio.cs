using System;
using System.Globalization;

namespace ImperitWASM.Shared.State
{
	[Newtonsoft.Json.JsonConverter(typeof(Cvt.NewtonsoftRatio))]
	[System.Text.Json.Serialization.JsonConverter(typeof(Cvt.SystemRatio))]
	public readonly struct Ratio : IEquatable<Ratio>, IComparable<Ratio>
	{
		static readonly Random rng = new Random();
		public static Ratio Zero { get; } = new Ratio(0);
		public static Ratio One { get; } = new Ratio(int.MaxValue);

		readonly int value;
		public Ratio(long p) => value = p < 0 ? 0 : p > int.MaxValue ? int.MaxValue : (int)p;
		public static Ratio operator +(Ratio x, Ratio y) => new Ratio((long)x.value + y.value);
		public static Ratio operator -(Ratio x, Ratio y) => new Ratio((long)x.value - y.value);
		public static Ratio operator *(Ratio x, long y) => new Ratio(x.value * y);
		public static Ratio operator /(Ratio x, long y) => new Ratio(x.value / y);
		public Ratio Adjust(long mul, long div) => new Ratio(value * mul / div);
		public static int operator *(int x, Ratio y) => y.Adjust(x, int.MaxValue).ToInt();
		public static int operator /(int x, Ratio y) => y.Adjust(int.MaxValue, x).ToInt();
		public static Ratio operator *(Ratio x, Ratio y) => x.Adjust(y.value, int.MaxValue);
		public static Ratio operator /(Ratio x, Ratio y) => x.Adjust(int.MaxValue, y.value);
		public static Ratio operator !(Ratio x) => new Ratio(int.MaxValue - x.value);
		public static Ratio operator |(Ratio x, Ratio y) => !(!x * !y);
		public static Ratio operator &(Ratio x, Ratio y) => x * y;
		public static Ratio operator ^(Ratio x, Ratio y) => (x | y) - (x & y);

		public static bool operator <(Ratio x, Ratio y) => x.value < y.value;
		public static bool operator >(Ratio x, Ratio y) => x.value > y.value;
		public static bool operator <=(Ratio x, Ratio y) => x.value <= y.value;
		public static bool operator >=(Ratio x, Ratio y) => x.value >= y.value;
		public static bool operator ==(Ratio x, Ratio y) => x.value == y.value;
		public static bool operator !=(Ratio x, Ratio y) => x.value != y.value;
		public override bool Equals(object? obj) => obj is Ratio p && Equals(p);
		public override int GetHashCode() => value.GetHashCode();
		public long ToUnits(long max) => value * max / int.MaxValue;
		public int ToInt() => value;
		public string ToString(string fmt, long units, long units2)
		{
			return string.Format(CultureInfo.InvariantCulture, fmt, ToUnits(units), ToUnits(units * units2) % units2);
		}
		public override string ToString() => ToString("{0}.{1} %", 100, 10);
		public int CompareTo(Ratio other) => value.CompareTo(other.value);
		public bool Equals(Ratio other) => value.Equals(other.value);
		public bool RandomBool => rng.Next() < value;
		public bool Any => value > 0;
	}
}
