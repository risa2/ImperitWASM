using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Soldiers : IEnumerable<Regiment>
	{
		ImmutableArray<Regiment> regiments;
		Soldiers(ImmutableArray<Regiment> regiments) => this.regiments = regiments;
		public Soldiers() => regiments = ImmutableArray<Regiment>.Empty;
		public Soldiers(IEnumerable<Regiment> list) => regiments = list.Where(r => r.Count > 0).ToImmutableArray();
		public Soldiers(SoldierType type, int count) => regiments = ImmutableArray.Create(new Regiment(type, count));

		public Soldiers Add(Soldiers s)
		{
			var res = regiments.ToBuilder();
			res.InsertMatch(s.Where(p => p.Count > 0), (a, b) => a.Type == b.Type, (a, b) => new Regiment(a.Type, a.Count + b.Count));
			return new Soldiers(res.ToImmutable());
		}
		public bool Contains(Soldiers s)
		{
			return s.regiments.All(s2 => regiments.Any(s1 => s1.Type == s2.Type && s1.Count >= s2.Count));
		}
		public Soldiers Subtract(Soldiers s)
		{
			var res = regiments.ToBuilder();
			res.InsertMatch(s.Where(p => p.Count > 0), (a, b) => a.Type == b.Type, (a, b) => new Regiment(a.Type, a.Count - b.Count));
			return new Soldiers(res.Where(x => x.Count > 0).ToImmutableArray());
		}
		public static Soldiers operator +(Soldiers s1, Soldiers s2) => s1.Add(s2);
		public static Soldiers operator -(Soldiers s1, Soldiers s2) => s1.Subtract(s2);
		public Soldiers Multiply(int mul) => new Soldiers(regiments.Select(s => new Regiment(s.Type, s.Count * mul)));
		public IEnumerable<SoldierType> Types => regiments.Select(p => p.Type);
		public int AttackPower => regiments.Sum(p => p.AttackPower);
		public int DefensePower => regiments.Sum(p => p.DefensePower);
		public int Power => regiments.Sum(p => p.Power);
		public int Weight => regiments.Sum(p => p.Weight);
		public int Price => regiments.Sum(p => p.Price);
		public int Count => regiments.Sum(p => p.Count);
		public bool Any => regiments.Any(p => p.Count > 0);

		public int Length => regiments.Length;
		public Regiment this[int index] => regiments[index];

		public int CountOf(SoldierType type) => regiments.Where(r => r.Type == type).Sum(r => r.Count);
		public int Capacity(Provinces provinces, Province from, Province to)
		{
			return regiments.Sum(p => p.CanMove(provinces, from, to) - p.Weight);
		}
		public bool CanMove(Provinces provinces, Province from, Province to)
		{
			return Any && from.Has(this) && Capacity(provinces, from, to) >= 0;
		}
		public int CanSustain(Region province) => regiments.Sum(p => p.CanSustain(province));
		public bool CanSurviveIn(Region province) => regiments.Sum(p => p.CanSustain(province) - p.Weight) >= 0;

		int[] Fight(int enemy, Func<SoldierType, int> powerof)
		{
			int me = regiments.Sum(regiment => powerof(regiment.Type) * regiment.Count), died = 0;
			int[] remaining = new int[regiments.Length];
			if (enemy > me)
			{
				return remaining;
			}
			for (int i = 0; i < remaining.Length; ++i)
			{
				if (powerof(regiments[i].Type) > 0)
				{
					remaining[i] = regiments[i].Count - (regiments[i].Count * enemy / me);
					died += (regiments[i].Count - remaining[i]) * powerof(regiments[i].Type);
				}
				else
				{
					remaining[i] = regiments[i].Count;
				}
			}
			for (int i = 0; died < enemy && i < remaining.Length; ++i)
			{
				if (powerof(regiments[i].Type) > 0)
				{
					int d = Math.Min(remaining[i], ((enemy - died) / powerof(regiments[i].Type)) + 1);
					remaining[i] -= d;
					died += d * powerof(regiments[i].Type);
				}

			}
			return remaining;
		}
		public Soldiers FightAgainst(int power, Func<SoldierType, int> powerof)
		{
			return new Soldiers(Fight(power, powerof).Select((count, i) => new Regiment(regiments[i].Type, count)).Where(pair => pair.Count > 0));
		}
		public Soldiers AttackedBy(Soldiers s2)
		{
			int defensePower = DefensePower, attackPower = s2.AttackPower;
			return defensePower >= attackPower
					? FightAgainst(attackPower, type => type.DefensePower)
					: s2.FightAgainst(defensePower, type => type.AttackPower);
		}
		public Soldiers MaxMovable(Provinces provinces, Province from, Province to)
		{
			var result = new Soldiers(regiments.Where(p => p.CanMoveAlone(provinces, from, to)));
			foreach (var (type, count) in regiments.Where(p => !p.CanMoveAlone(provinces, from, to)).OrderBy(p => p.Type.Weight - p.Type.CanMove(provinces, from, to)))
			{
				result += new Soldiers(type, Math.Min(result.Capacity(provinces, from, to) / (type.Weight - type.CanMove(provinces, from, to)), count));
			}
			return result;
		}
		public Soldiers MaxPersistent(Region where)
		{
			var result = new Soldiers(regiments.Where(r => r.CanSustain(where) >= r.Weight));
			foreach (var (type, count) in regiments.Where(r => r.CanSustain(where) < r.Weight).OrderBy(r => r.Weight - r.CanSustain(where)))
			{
				result += new Soldiers(type, Math.Min(result.CanSustain(where) / (type.Weight - type.CanSustain(where)), count));
			}
			return result;
		}

		public IEnumerator<Regiment> GetEnumerator() => (regiments as IEnumerable<Regiment>).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public override string ToString() => string.Join("", regiments.Select(p => p.Count + p.Type.Symbol));
	}
}
