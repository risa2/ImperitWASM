using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Soldiers(ImmutableArray<Regiment> Regiments) : IEnumerable<Regiment>
	{
		public Soldiers() : this(ImmutableArray<Regiment>.Empty) { }
		public Soldiers(IEnumerable<Regiment> list) : this(list.ToImmutableArray()) { }
		public Soldiers(params Regiment[] list) : this(list.ToImmutableArray()) { }
		public Soldiers(SoldierType type, int count) : this(ImmutableArray.Create(new Regiment(type, count))) { }
		public Soldiers Add(Soldiers s)
		{
			var res = Regiments.ToBuilder();
			res.InsertMatch(s.Where(p => p.Count > 0), (a, b) => a.Type == b.Type, (a, b) => new Regiment(a.Type, a.Count + b.Count));
			return new Soldiers(res.ToImmutable());
		}
		public bool Contains(Soldiers s)
		{
			return s.Regiments.All(s2 => Regiments.Any(s1 => s1.Type == s2.Type && s1.Count >= s2.Count));
		}
		public Soldiers Subtract(Soldiers s)
		{
			var res = Regiments.ToBuilder();
			res.InsertMatch(s.Where(p => p.Count > 0), (a, b) => a.Type == b.Type, (a, b) => new Regiment(a.Type, a.Count - b.Count));
			return new Soldiers(res.Where(x => x.Count > 0).ToImmutableArray());
		}
		public static Soldiers operator +(Soldiers s1, Soldiers s2) => s1.Add(s2);
		public static Soldiers operator -(Soldiers s1, Soldiers s2) => s1.Subtract(s2);
		public Soldiers Multiply(int mul) => new Soldiers(Regiments.Select(s => new Regiment(s.Type, s.Count * mul)));
		public IEnumerable<SoldierType> Types => Regiments.Select(p => p.Type);
		public int AttackPower => Regiments.Sum(p => p.AttackPower);
		public int DefensePower => Regiments.Sum(p => p.DefensePower);
		public int Power => Regiments.Sum(p => p.Power);
		public int Weight => Regiments.Sum(p => p.Weight);
		public int Price => Regiments.Sum(p => p.Price);
		public int Count => Regiments.Sum(p => p.Count);
		public bool Any => Regiments.Any(p => p.Count > 0);

		public int Length => Regiments.Length;
		public Regiment this[int index] => Regiments[index];

		public int Capacity(PlayersAndProvinces pap, Province from, Province to)
		{
			return Regiments.Sum(p => p.CanMove(pap, from, to) - p.Weight);
		}
		public bool CanMove(PlayersAndProvinces pap, Province from, Province to)
		{
			return Any && from.Has(this) && Capacity(pap, from, to) >= 0;
		}
		public bool CanSurviveIn(Province province) => Regiments.Sum(p => p.CanSustain(province) - p.Weight) >= 0;

		int[] Fight(int me, int enemy, Func<SoldierType, int> powerof)
		{
			int died = 0;
			int[] remaining = new int[Regiments.Length];
			for (int i = 0; i < remaining.Length; ++i)
			{
				if (powerof(Regiments[i].Type) > 0)
				{
					remaining[i] = Regiments[i].Count - (Regiments[i].Count * enemy / me);
					died += (Regiments[i].Count - remaining[i]) * powerof(Regiments[i].Type);
				}
				else
				{
					remaining[i] = Regiments[i].Count;
				}
			}
			for (int i = 0; died < enemy && i < remaining.Length; ++i)
			{
				if (powerof(Regiments[i].Type) > 0)
				{
					int d = Math.Min(remaining[i], ((enemy - died) / powerof(Regiments[i].Type)) + 1);
					remaining[i] -= d;
					died += d * powerof(Regiments[i].Type);
				}

			}
			return remaining;
		}
		public Soldiers AttackedBy(int power) => new Soldiers(Fight(DefensePower, power, type => type.DefensePower).Select((count, i) => new Regiment(Regiments[i].Type, count)).Where(pair => pair.Count > 0));
		public Soldiers AttackedBy(Soldiers s2)
		{
			int defensePower = DefensePower, attackPower = s2.AttackPower;
			var (s, power1, power2, powerof) = defensePower >= attackPower
					? (this, defensePower, attackPower, (Func<SoldierType, int>)(type => type.DefensePower))
					: (s2, attackPower, defensePower, type => type.AttackPower);
			var remaining = s.Fight(power1, power2, powerof).Select((count, i) => new Regiment(s[i].Type, count));
			return new Soldiers(remaining.Where(pair => pair.Count > 0));
		}
		public Soldiers MaxAttackers(PlayersAndProvinces pap, Province from, Province to)
		{
			var result = new Soldiers(Regiments.Where(p => p.CanMoveAlone(pap, from, to)));
			foreach (var p in Regiments.Where(p => !p.CanMoveAlone(pap, from, to)).OrderBy(p => p.Type.Weight - p.Type.CanMove(pap, from, to)))
			{
				result += new Soldiers(p.Type, Math.Min(result.Capacity(pap, from, to) / (p.Type.Weight - p.Type.CanMove(pap, from, to)), p.Count));
			}
			return result;
		}
		public IEnumerator<Regiment> GetEnumerator() => (Regiments as IEnumerable<Regiment>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public override string ToString() => string.Join("", Regiments.Select(p => p.Count + p.Type.Symbol));
	}
}
