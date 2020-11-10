using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Cmd;
using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Func;
using static System.Math;

namespace ImperitWASM.Shared.Entities
{
	public class Robot : Player
	{
		public Robot(Color color, int money, bool alive, ImmutableList<PlayerAction> actions)
			: base(color, new Description(), money, alive, actions) { }
		public override Player ChangeMoney(int amount) => new Robot(Color, Money + amount, Alive, Actions);
		public override Player Die() => new Robot(Color, 0, false, ImmutableList<PlayerAction>.Empty);
		protected override Player WithActions(ImmutableList<PlayerAction> new_actions) => new Robot(Color, Money, Alive, new_actions);

		static Soldiers NextSoldiers(PlayersAndProvinces pap, Province p, Settings set) => p.ActOnYourself(set, pap).Soldiers;
		static int NextDefensePower(PlayersAndProvinces pap, Province p, Settings set) => NextSoldiers(pap, p, set).DefensePower;
		static int NextAttackPower(PlayersAndProvinces pap, Province p, Settings set) => NextSoldiers(pap, p, set).AttackPower;
		bool IsEnemy(Province p) => !p.IsAllyOf(this) && p.Occupied;
		IEnumerable<Province> EnemyNeighbors(PlayersAndProvinces pap, Province prov) => pap.NeighborsOf(prov).Where(IsEnemy);
		int EnemiesPower(PlayersAndProvinces pap, Province prov, Settings set) => EnemyNeighbors(pap, prov).GroupBy(p => p.Player).Max(p => p.Sum(neighbor => NextAttackPower(pap, neighbor, set)));
		int Bilance(PlayersAndProvinces pap, Province prov, Settings set) => NextDefensePower(pap, prov, set) - EnemiesPower(pap, prov, set);
		void Recruit(ref PlayersAndProvinces pap, ref int spent, Province province, Soldiers soldiers)
		{
			(pap, _) = pap.Do(new Recruit(this, province, soldiers));
			spent += soldiers.Price;
		}
		SoldierType? BestDefender(Province p, int money, Settings set)
		{
			return set.SoldierTypes.Where(type => type.IsRecruitable(p)).MinBy(type => -money / type.Price * type.DefensePower);
		}
		static int DivUp(int a, int b) => (a / b) + (a % b > 0 ? 1 : 0);
		void DefensiveRecruits(ref PlayersAndProvinces pap, ref int spent, Province[] my, Settings set)
		{
			foreach (var place in my)
			{
				if (EnemiesPower(pap, place, set) - NextDefensePower(pap, place, set) is int shortage && shortage > 0 && BestDefender(place, Money - spent, set) is SoldierType type && shortage <= (Money - spent) / type.Price * type.DefensePower)
				{
					Recruit(ref pap, ref spent, place, Soldiers.From(type, DivUp(shortage, type.DefensePower)));
				}
			}
		}
		void StabilisatingRecruits(ref PlayersAndProvinces pap, ref int spent, Province[] my, Settings set)
		{
			foreach (var place in my)
			{
				if (Money <= spent)
				{
					break;
				}
				if (place.DefaultSoldiers.DefensePower - NextDefensePower(pap, place, set) is int shortage && shortage > 0 && BestDefender(place, Money - spent, set) is SoldierType type)
				{
					Recruit(ref pap, ref spent, place, Soldiers.From(type, Min((Money - spent) / type.Price, DivUp(shortage, type.DefensePower))));
				}
			}
		}
		void Recruits(ref PlayersAndProvinces pap, Province[] my, Settings set)
		{
			int spent = 0;
			DefensiveRecruits(ref pap, ref spent, my, set);
			StabilisatingRecruits(ref pap, ref spent, my, set);
			if (my.Any() && BestDefender(my[0], Money - spent, set) is SoldierType type && Money - spent > 0)
			{
				Recruit(ref pap, ref spent, my[0], Soldiers.From(type, (Money - spent) / type.Price));
			}
		}
		static bool CanConquer(Soldiers from, Soldiers to) => from.AttackPower > to.DefensePower;
		static bool CanKeepConqueredProvince(Soldiers from, Soldiers to, int enemies_to) => from.AttackPower >= to.DefensePower + enemies_to;
		static bool CanAttackSuccesfully(Soldiers attackers, Soldiers defenders, int enemies_to) => CanConquer(attackers, defenders) && CanKeepConqueredProvince(attackers, defenders, enemies_to);
		bool ShouldAttack(Soldiers attackers, Province to, int enemies) => !to.IsAllyOf(this) && CanAttackSuccesfully(attackers, to.Soldiers, enemies);
		void Attack(ref PlayersAndProvinces pap, Province from, Province to, Soldiers soldiers)
		{
			(pap, _) = pap.Do(new Move(this, from, to, soldiers));
		}
		IEnumerable<(Province, Soldiers)> GetAttacks(PlayersAndProvinces pap, Province from, Settings set)
		{
			int enemies = EnemiesPower(pap, from, set);
			return pap.NeighborsOf(from).Select(to => (to, from.Soldiers.MaxAttackers(pap, from, to).AttackedBy(enemies - (IsEnemy(to) ? to.Soldiers.AttackPower : 0)) ?? Soldiers.Empty)).Where(to => ShouldAttack(to.Item2, to.to, EnemiesPower(pap, to.to, set)));
		}
		void Attacks(ref PlayersAndProvinces pap, Province[] my, Settings set)
		{
			foreach (var from in my)
			{
				foreach (var (to, soldiers) in GetAttacks(pap, from, set))
				{
					Attack(ref pap, from, to, soldiers);
				}
			}
		}
		IEnumerable<Province> NeighborAllies(PlayersAndProvinces pap, Province pr) => pap.NeighborsOf(pr).Where(n => n.IsAllyOf(this));
		IEnumerable<Province> AttackPlaces(PlayersAndProvinces pap, Province[] my) => my.SelectMany(place => pap.NeighborsOf(place).Where(p => p.Occupied && !p.IsAllyOf(this)));
		Soldiers AttackersFrom(PlayersAndProvinces pap, IEnumerable<Province> starts, Province to, Settings set)
		{
			return starts.Aggregate(Soldiers.Empty, (s, from) => s.Add(from.Soldiers.MaxAttackers(pap, from, to).AttackedBy(EnemiesPower(pap, from, set) - to.Soldiers.AttackPower) ?? Soldiers.Empty));
		}
		void MultiAttacks(ref PlayersAndProvinces pap, Province[] my, Settings set)
		{
			foreach (var to in AttackPlaces(pap, my).Distinct())
			{
				var starts = NeighborAllies(pap, to);
				int total = AttackersFrom(pap, starts, to, set).AttackPower;
				if (total > to.Soldiers.DefensePower + EnemiesPower(pap, to, set))
				{
					foreach (var from in starts)
					{
						Attack(ref pap, from, to, from.Soldiers.MaxAttackers(pap, from, to).AttackedBy(EnemiesPower(pap, from, set) - to.Soldiers.AttackPower) ?? Soldiers.Empty);
					}
				}
			}
		}
		void Transport(ref PlayersAndProvinces pap, Province from, Province to, Soldiers soldiers)
		{
			(pap, _) = pap.Do(new Move(this, from, to, soldiers));
		}
		void SpreadSoldiers(ref PlayersAndProvinces pap, Province[] my, Settings set)
		{
			foreach (var from in my)
			{
				int enemies_from = EnemiesPower(pap, from, set);
				int bilance_from = NextDefensePower(pap, from, set) - enemies_from;
				var p_p = pap;
				foreach (var (dest, bilance) in pap.NeighborsOf(from).Where(n => n.IsAllyOf(this)).Select(p => (p, Bilance(p_p, p, set))).OrderBy(n => n.Item2))
				{
					if (enemies_from <= 0 && EnemyNeighbors(pap, dest).Any())
					{
						Transport(ref pap, from, dest, from.Soldiers);
					}
					else if (bilance_from > 0 && bilance_from >= bilance && from.Soldiers.DefensePower >= from.DefaultSoldiers.DefensePower)
					{
						Transport(ref pap, from, dest, from.Soldiers.AttackedBy(from.DefaultSoldiers));
					}
				}
			}
		}
		public PlayersAndProvinces Think(PlayersAndProvinces pap, Settings set)
		{
			var my = pap.Provinces.ControlledBy(this).ToArray();
			Recruits(ref pap, my, set);
			Attacks(ref pap, my, set);
			MultiAttacks(ref pap, my, set);
			SpreadSoldiers(ref pap, my, set);
			return pap;
		}
	}
}
