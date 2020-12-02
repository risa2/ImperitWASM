using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.Motion.Commands;
using static System.Math;

namespace ImperitWASM.Shared.State
{
	public record Robot(Color Color, string Name, int Money, bool Alive, ImmutableList<IPlayerAction> Actions, Settings Settings)
		: Player(Color, Name, Money, Alive, Actions)
	{
		public virtual bool Equals(Robot? obj) => obj is not null && obj.Name == Name;
		public override int GetHashCode() => base.GetHashCode();
		public override Player Die() => new Robot(Color, Name, 0, false, ImmutableList<IPlayerAction>.Empty, Settings);

		static int NextDefensePower(PlayersAndProvinces pap, Province p) => p.NextSoldiers(pap).DefensePower;
		static int NextAttackPower(PlayersAndProvinces pap, Province p) => p.NextSoldiers(pap).AttackPower;
		IEnumerable<Province> EnemyNeighbors(PlayersAndProvinces pap, Province prov) => pap.NeighborsOf(prov).Where(p => p.IsEnemyOf(this));
		int EnemiesPower(PlayersAndProvinces pap, Province prov) => EnemyNeighbors(pap, prov).GroupBy(p => p.Player).Select(p => p.Sum(neighbor => NextAttackPower(pap, neighbor))).DefaultIfEmpty().Max();
		int Bilance(PlayersAndProvinces pap, Province prov) => NextDefensePower(pap, prov) - EnemiesPower(pap, prov);

		PlayersAndProvinces Recruit(PlayersAndProvinces pap, ref int spent, int province, Soldiers soldiers)
		{
			spent += soldiers.Price;
			return pap.JustAdd(new Recruit(this, pap.Province(province), soldiers));
		}

		PlayersAndProvinces Move(PlayersAndProvinces pap, int from, int to, Soldiers soldiers)
		{
			return pap.JustAdd(new Move(this, pap.Province(from), pap.Province(to), soldiers));
		}

		SoldierType? BestDefender(Province p, int money)
		{
			return Settings.RecruitableTypes(p).MinBy(type => -money / type.Price * type.DefensePower);
		}

		static int DivUp(int a, int b) => (a + b - 1) / b;

		(Soldiers, bool) SoldiersRequired(PlayersAndProvinces pap, int money, Province place, int desired) => desired - NextDefensePower(pap, place) is int shortage && shortage > 0 && BestDefender(place, money) is SoldierType type ? (new Soldiers(type, Min(money / type.Price, DivUp(shortage, type.DefensePower))), shortage <= money / type.Price * type.DefensePower) : (new Soldiers(), false);

		(PlayersAndProvinces, int) DefensiveRecruits(PlayersAndProvinces pap, int spent, ImmutableArray<int> my)
		{
			foreach (int place in my)
			{
				if (SoldiersRequired(pap, Money - spent, pap.Province(place), EnemiesPower(pap, pap.Province(place))) is (Soldiers soldiers, true))
				{
					pap = Recruit(pap, ref spent, place, soldiers);
				}
			}
			return (pap, spent);
		}

		(PlayersAndProvinces, int) StabilisatingRecruits(PlayersAndProvinces pap, int spent, ImmutableArray<int> my)
		{
			foreach (int place in my)
			{
				if (Money <= spent)
				{
					break;
				}
				if (SoldiersRequired(pap, Money - spent, pap.Province(place), EnemiesPower(pap, pap.Province(place))) is (Soldiers soldiers, _))
				{
					pap = Recruit(pap, ref spent, place, soldiers);
				}
			}
			return (pap, spent);
		}

		PlayersAndProvinces Recruits(PlayersAndProvinces pap, ImmutableArray<int> my)
		{
			int spent = 0;
			(pap, spent) = DefensiveRecruits(pap, spent, my);
			(pap, spent) = StabilisatingRecruits(pap, spent, my);
			return my.Any() && BestDefender(pap.Province(my[0]), Money - spent) is SoldierType type && Money - spent > 0 ? Recruit(pap, ref spent, my[0], new Soldiers(type, (Money - spent) / type.Price)) : pap;
		}

		static bool CanConquer(Soldiers from, Soldiers to) => from.AttackPower > to.DefensePower;
		static bool CanKeepConqueredProvince(Soldiers from, Soldiers to, int enemies_to) => from.AttackPower >= to.DefensePower + enemies_to;
		static bool CanAttackSuccesfully(Soldiers attackers, Soldiers defenders, int enemies_to) => CanConquer(attackers, defenders) && CanKeepConqueredProvince(attackers, defenders, enemies_to);
		bool ShouldAttack(Soldiers attackers, Province to, int enemies) => !to.IsAllyOf(this) && CanAttackSuccesfully(attackers, to.Soldiers, enemies);

		static Soldiers Units(int num) => new Soldiers(new Pedestrian(new Description("", ImmutableArray<string>.Empty), 1, 1, 1, 1), num);

		IEnumerable<(int, Soldiers)> GetAttacks(PlayersAndProvinces pap, Province from)
		{
			int enemies = EnemiesPower(pap, from);
			return pap.NeighborIndices(from).Select(to => (to, from.MaxAttackers(pap, pap.Province(to)).AttackedBy(Units(enemies - (pap.Province(to).IsEnemyOf(this) ? pap.Province(to).AttackPower : 0))))).Where(to => ShouldAttack(to.Item2, pap.Province(to.to), EnemiesPower(pap, pap.Province(to.to))));
		}

		PlayersAndProvinces Attacks(PlayersAndProvinces pap, ImmutableArray<int> my)
		{
			foreach (int from in my)
			{
				foreach (var (to, soldiers) in GetAttacks(pap, pap.Province(from)))
				{
					pap = Move(pap, from, to, soldiers);
				}
			}
			return pap;
		}

		IEnumerable<int> NeighborAllies(PlayersAndProvinces pap, int i) => pap.NeighborIndices(pap.Province(i)).Where(n => pap.Province(n).IsAllyOf(this));
		IEnumerable<int> AttackPlaces(PlayersAndProvinces pap, ImmutableArray<int> my) => my.SelectMany(place => pap.NeighborIndices(pap.Province(place)).Where(p => pap.Province(p).IsEnemyOf(this)));

		Soldiers AttackersFrom(PlayersAndProvinces pap, IEnumerable<int> starts, int to)
		{
			return starts.Aggregate(new Soldiers(), (s, from) => s.Add(pap.Province(from).MaxAttackers(pap, pap.Province(to)).AttackedBy(Units(EnemiesPower(pap, pap.Province(from)) - pap.Province(to).Soldiers.AttackPower))));
		}

		PlayersAndProvinces MultiAttacks(PlayersAndProvinces pap, ImmutableArray<int> my)
		{
			foreach (int to in AttackPlaces(pap, my).Distinct())
			{
				var starts = NeighborAllies(pap, to);
				int total = AttackersFrom(pap, starts, to).AttackPower;
				if (total > pap.Province(to).DefensePower + EnemiesPower(pap, pap.Province(to)))
				{
					foreach (int from in starts)
					{
						pap = Move(pap, from, to, pap.Province(from).MaxAttackers(pap, pap.Province(to)).AttackedBy(Units(EnemiesPower(pap, pap.Province(from)) - pap.Province(to).AttackPower)));
					}
				}
			}
			return pap;
		}

		PlayersAndProvinces SpreadSoldiers(PlayersAndProvinces pap, ImmutableArray<int> my)
		{
			foreach (int from in my)
			{
				int enemies_from = EnemiesPower(pap, pap.Province(from));
				int bilance_from = NextDefensePower(pap, pap.Province(from)) - enemies_from;
				foreach (var (dest, bilance) in pap.NeighborIndices(pap.Province(from)).Where(n => pap.Province(n).IsAllyOf(this)).Select(p => (p, Bilance(pap, pap.Province(p)))).OrderBy(n => n.Item2))
				{
					if (enemies_from <= 0 && EnemyNeighbors(pap, pap.Province(dest)).Any())
					{
						pap = Move(pap, from, dest, pap.Province(from).Soldiers);
					}
					else if (bilance_from > 0 && bilance_from >= bilance && pap.Province(from).DefensePower >= pap.Province(from).DefaultDefensePower)
					{
						pap = Move(pap, from, dest, pap.Province(from).Soldiers.AttackedBy(pap.Province(from).DefaultSoldiers));
					}
				}
			}
			return pap;
		}
		public PlayersAndProvinces Think(PlayersAndProvinces pap)
		{
			var my = pap.Provinces.Indices(p => p.IsAllyOf(this)).ToImmutableArray();
			return SpreadSoldiers(MultiAttacks(Attacks(Recruits(pap, my), my), my), my);
		}
	}
}
