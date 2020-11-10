using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class Soldiers : IReadOnlyList<Regiment>
	{
		private int _id;
		public IReadOnlyList<Regiment> Regiments { get; }
		public Soldiers(IReadOnlyList<Regiment> list) => Regiments = list;
		public static Soldiers Empty { get; } = new Soldiers(Array.Empty<Regiment>());
		public static Soldiers From(SoldierType type, int count) => new Soldiers(new[] { new Regiment(type, count) });
		public Soldiers Add(Soldiers s)
		{
			var res = Regiments.ToList();
			res.InsertMatch(s.Where(p => p.Count > 0), a => a.Type, (a, b) => new Regiment(a.Type, a.Count + b.Count));
			return new Soldiers(res);
		}
		public bool Contains(Soldiers s)
		{
			return s.Regiments.All(s2 => Regiments.Any(s1 => s1.Type == s2.Type && s1.Count >= s2.Count));
		}
		public Soldiers Subtract(Soldiers s)
		{
			var res = Regiments.ToList();
			res.InsertMatch(s.Where(p => p.Count > 0), a => a.Type, (a, b) => new Regiment(a.Type, a.Count - b.Count));
			return new Soldiers(res.Where(x => x.Count > 0).ToArray());
		}
		public Soldiers Multiply(int mul) => new Soldiers(Regiments.Select(s => new Regiment(s.Type, s.Count * mul)).ToArray());
		public int AttackPower => Regiments.Sum(p => p.Count * p.Type.AttackPower);
		public int DefensePower => Regiments.Sum(p => p.Count * p.Type.DefensePower);
		public int Power => Regiments.Sum(p => p.Count * p.Type.Power);
		public int Weight => Regiments.Sum(p => p.Count * p.Type.Weight);
		public int Price => Regiments.Sum(p => p.Count * p.Type.Price);
		public int UnitCount => Regiments.Sum(p => p.Count);
		public bool Any => Regiments.Any(p => p.Count > 0);

		public int Count => Regiments.Count;
		public Regiment this[int index] => Regiments[index];
		public IEnumerator<Regiment> GetEnumerator() => Regiments.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public static Soldiers operator +(Soldiers s1, Soldiers s2) => s1.Add(s2);
		public static Soldiers operator -(Soldiers s1, Soldiers s2) => s1.Subtract(s2);

		public int Capacity(PlayersAndProvinces pap, Province from, Province to)
		{
			return Regiments.Sum(p => p.Count * (p.Type.CanMove(pap, from, to) - p.Type.Weight));
		}
		public bool CanMove(PlayersAndProvinces pap, Province from, Province to)
		{
			return Any && from.Soldiers.Contains(this) && Capacity(pap, from, to) >= 0;
		}
		public bool CanSurviveIn(Province province) => Regiments.Sum(p => (p.Type.CanSustain(province) - p.Type.Weight) * p.Count) >= 0;

		static int[] Fight(Soldiers soldiers, int me, int enemy, Func<SoldierType, int> powerof)
		{
			int died = 0;
			int[] remaining = new int[soldiers.Count];
			for (int i = 0; i < remaining.Length; ++i)
			{
				if (powerof(soldiers[i].Type) > 0)
				{
					remaining[i] = soldiers[i].Count - (soldiers[i].Count * enemy / me);
					died += (soldiers[i].Count - remaining[i]) * powerof(soldiers[i].Type);
				}
				else
				{
					remaining[i] = soldiers[i].Count;
				}
			}
			for (int i = 0; died < enemy && i < remaining.Length; ++i)
			{
				if (powerof(soldiers[i].Type) > 0)
				{
					var d = Math.Min(remaining[i], ((enemy - died) / powerof(soldiers[i].Type)) + 1);
					remaining[i] -= d;
					died += d * powerof(soldiers[i].Type);
				}

			}
			return remaining;
		}
		public Soldiers? AttackedBy(int attackpower)
		{
			int defensePower = DefensePower;
			if (defensePower < attackpower)
			{
				return null;
			}
			var remaining = Fight(this, defensePower, attackpower, type => type.DefensePower);
			return new Soldiers(remaining.Where(c => c > 0).Select((count, i) => new Regiment(Regiments[i].Type, count)).ToArray());
		}
		public Soldiers AttackedBy(Soldiers s2)
		{
			int defensePower = DefensePower, attackPower = s2.AttackPower;
			var (s, power1, power2, powerof) = defensePower >= attackPower
					? (this, defensePower, attackPower, (Func<SoldierType, int>)(type => type.DefensePower))
					: (s2, attackPower, defensePower, type => type.AttackPower);
			var remaining = Fight(s, power1, power2, powerof);
			return new Soldiers(remaining.Where(c => c > 0).Select((count, i) => new Regiment(s[i].Type, count)).ToArray());
		}
		public T AttackedBy<T>(Soldiers s2, Func<Soldiers, T> me, Func<Soldiers, T> enemy) => DefensePower < s2.AttackPower ? enemy(AttackedBy(s2)) : me(AttackedBy(s2));
		public Soldiers MaxAttackers(PlayersAndProvinces pap, Province from, Province to)
		{
			var result = new Soldiers(Regiments.Where(p => p.Type.CanMoveAlone(pap, from, to)).ToArray());
			foreach (var p in Regiments.Where(p => !p.Type.CanMoveAlone(pap, from, to)).OrderBy(p => p.Type.Weight - p.Type.CanMove(pap, from, to)))
			{
				result += new Soldiers(new[] { new Regiment(p.Type, Math.Min(result.Capacity(pap, from, to) / (p.Type.Weight - p.Type.CanMove(pap, from, to)), p.Count)) });
			}
			return result;
		}
		public override string ToString() => string.Join("", Regiments.Select(p => p.Count + p.Type.Symbol));
	}
}
