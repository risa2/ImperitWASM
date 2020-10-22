using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Conversion.ProbabilityConverter))]
	public readonly struct Probability : IEquatable<Probability>, IComparable<Probability>
	{
		static readonly Random rng = new Random();
		public static Probability Impossible { get; } = new Probability(0);
		public static Probability Certain { get; } = new Probability(int.MaxValue);

		readonly int prob;
		public Probability(long p) => prob = p < 0 ? 0 : p > int.MaxValue ? int.MaxValue : (int)p;
		public static Probability operator +(Probability x, Probability y) => new Probability((long)x.prob + y.prob);
		public static Probability operator -(Probability x, Probability y) => new Probability((long)x.prob - y.prob);
		public static Probability operator *(Probability x, long y) => new Probability(x.prob * y);
		public static Probability operator /(Probability x, long y) => new Probability(x.prob / y);
		public Probability Adjust(long mul, long div) => new Probability(prob * mul / div);
		public static bool operator <(Probability x, Probability y) => x.prob < y.prob;
		public static bool operator >(Probability x, Probability y) => x.prob > y.prob;
		public static bool operator <=(Probability x, Probability y) => x.prob <= y.prob;
		public static bool operator >=(Probability x, Probability y) => x.prob >= y.prob;
		public static bool operator ==(Probability x, Probability y) => x.prob == y.prob;
		public static bool operator !=(Probability x, Probability y) => x.prob != y.prob;
		public override bool Equals(object? obj) => obj is Probability p && Equals(p);
		public override int GetHashCode() => prob.GetHashCode();
		public int ToUnits(int max) => prob * max / int.MaxValue;
		public long ToUnits(long max) => prob * max / int.MaxValue;
		public string ToString(string fmt, int units, int units2)
		{
			return string.Format(CultureInfo.InvariantCulture, fmt, ToUnits(units), ToUnits(units * units2) % units2);
		}
		public override string ToString() => ToString("{0}.{1} %", 100, 10);
		public int CompareTo(Probability other) => prob.CompareTo(other.prob);
		public bool Equals(Probability other) => prob.Equals(other.prob);
		public bool Next => rng.Next() < prob;
		public bool Possible => prob > 0;
	}
}
