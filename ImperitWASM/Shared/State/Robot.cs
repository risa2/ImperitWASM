using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.Motion.Commands;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static System.Math;

namespace ImperitWASM.Shared.State
{
	public class Robot : Player
	{
		readonly Settings settings;
		public Robot(int id, Color color, int money, bool alive, ImmutableList<IPlayerAction> actions, Settings settings)
			: base(id, color, money, alive, actions) => this.settings = settings;
		public override Player ChangeMoney(int amount) => new Robot(Id, Color, Money + amount, Alive, Actions, settings);
		public override Player Die() => new Robot(Id, Color, 0, false, ImmutableList<IPlayerAction>.Empty, settings);
		protected override Player WithActions(ImmutableList<IPlayerAction> new_actions) => new Robot(Id, Color, Money, Alive, new_actions, settings);
		class PInfo
		{
			public Soldiers Soldiers;
			public readonly bool Ally, Enemy;
			public int Enemies, Coming;
			public PInfo(Soldiers soldiers, bool ally, bool enemy, int enemies, int coming)
			{
				Soldiers = soldiers;
				Ally = ally;
				Enemy = enemy;
				Enemies = enemies;
				Coming = coming;
			}
			public int AttackPower => Soldiers.AttackPower;
			public int DefensePower => Soldiers.DefensePower;
			public int NextDefensePower => DefensePower + Coming;
			public int Bilance => ((Ally ? 1 : -1) * DefensePower) + Coming - Enemies;
		}
		IEnumerable<Province> NeighborEnemies(PlayersAndProvinces pap, Province prov) => pap.NeighborsOf(prov).Where(neighbor => !neighbor.IsAllyOf(this) && neighbor.Occupied);
		int EnemiesCount(PlayersAndProvinces pap, Province prov) => NeighborEnemies(pap, prov).Sum(neighbor => neighbor.Soldiers.AttackPower);
		void Recruit(ref PlayersAndProvinces pap, ref int spent, Province land, PInfo[] info, Soldiers soldiers)
		{
			info[land.Id].Coming += soldiers.DefensePower;
			(pap, _) = pap.Do(new Recruit(this, land, soldiers));
			spent += soldiers.Price;
		}
		SoldierType? BestDefender(Province p, int money)
		{
			return settings.SoldierTypes.Where(type => type.IsRecruitable(p)).MinBy(type => -money / type.Price * type.DefensePower);
		}
		static int DivUp(int a, int b) => (a / b) + (a % b > 0 ? 1 : 0);
		void DefensiveRecruits(ref PlayersAndProvinces pap, ref int spent, PInfo[] info, Province[] my)
		{
			foreach (var l in my)
			{
				if (BestDefender(l, Money - spent) is SoldierType type && info[l.Id].Bilance < 0 && info[l.Id].Bilance + ((Money - spent) / type.Price * type.DefensePower) >= 0)
				{
					Recruit(ref pap, ref spent, l, info, new Soldiers(type, DivUp(-info[l.Id].Bilance, type.DefensePower)));
				}
			}
		}
		void StabilisatingRecruits(ref PlayersAndProvinces pap, ref int spent, PInfo[] info, Province[] my)
		{
			foreach (var l in my)
			{
				if (Money <= spent)
				{
					break;
				}
				if (BestDefender(l, Money - spent) is SoldierType type && info[l.Id].NextDefensePower < l.DefaultSoldiers.DefensePower)
				{
					Recruit(ref pap, ref spent, l, info, new Soldiers(type, Min((Money - spent) / type.Price, DivUp(l.DefaultSoldiers.DefensePower - info[l.Id].NextDefensePower, type.DefensePower))));
				}
			}
		}
		void Recruits(ref PlayersAndProvinces pap, PInfo[] info, Province[] my)
		{
			int spent = 0;
			DefensiveRecruits(ref pap, ref spent, info, my);
			StabilisatingRecruits(ref pap, ref spent, info, my);
			if (my.Any() && BestDefender(my[0], Money - spent) is SoldierType type && Money - spent > 0)
			{
				Recruit(ref pap, ref spent, my[0], info, new Soldiers(type, (Money - spent) / type.Price));
			}
		}
		static bool CanConquer(Soldiers from, Soldiers to) => from.AttackPower > to.DefensePower;
		static bool CanKeepConqueredProvince(Soldiers from, Soldiers to, int enemies_to) => from.AttackPower >= to.DefensePower + enemies_to;
		static bool CanAttackSuccesfully(Soldiers attackers, Soldiers defenders, int enemies_to) => CanConquer(attackers, defenders) && CanKeepConqueredProvince(attackers, defenders, enemies_to);
		static bool ShouldAttack(Soldiers attackers, PInfo to) => !to.Ally && CanAttackSuccesfully(attackers, to.Soldiers, to.Enemies);
		void Attack(ref PlayersAndProvinces pap, PInfo[] info, Province from, Province to, Soldiers soldiers)
		{
			(pap, _) = pap.Do(new Move(this, from, to, new Army(soldiers, this)));
			info[from.Id].Soldiers -= soldiers;
			if (pap.Province(to.Id).Occupied && soldiers.AttackPower >= pap.Province(to.Id).Soldiers.DefensePower)
			{
				foreach (var neighbor in pap.NeighborsOf(to).Where(n => n.IsAllyOf(this)))
				{
					info[neighbor.Id].Enemies -= pap.Province(to.Id).Soldiers.AttackPower;
				}
			}
		}
		static readonly Soldiers Unit = new Soldiers(new SoldierTypes.Pedestrian(0, new Description("", "", ""), 1, 1, 1, 1), 1);
		IEnumerable<(int, Soldiers)> GetAttacks(PlayersAndProvinces pap, PInfo[] info, Province from)
		{
			return pap.NeighborsOf(from).Select(to => (to.Id, info[from.Id].Soldiers.MaxAttackers(pap, from.Id, to.Id).AttackedBy(Unit.Multiply(info[from.Id].Enemies - (info[to.Id].Enemy ? info[to.Id].AttackPower : 0))))).Where(to => ShouldAttack(to.Item2, info[to.Id]));
		}
		void Attacks(ref PlayersAndProvinces pap, PInfo[] info, int[] my)
		{
			foreach (int from in my)
			{
				foreach (var (to, soldiers) in GetAttacks(pap, info, pap.Province(from)))
				{
					Attack(ref pap, info, pap.Province(from), pap.Province(to), soldiers);
				}
			}
		}
		IEnumerable<Province> NeighborAllies(PlayersAndProvinces pap, int i) => pap.NeighborsOf(pap.Province(i)).Where(n => n.IsAllyOf(this));
		IEnumerable<int> AttackPlaces(PlayersAndProvinces pap, int[] my) => my.SelectMany(i => pap.NeighborsOf(pap.Province(i)).Where(p => p.Occupied && !p.IsAllyOf(this)).Select(p => p.Id));
		Soldiers AttackersFrom(PlayersAndProvinces pap, PInfo[] info, IEnumerable<Province> starts, int to) => starts.Aggregate(new Soldiers(), (s, from) => s.Add(from.Soldiers.MaxAttackers(pap, from.Id, to).AttackedBy(Unit.Multiply(info[from.Id].Enemies - info[to].AttackPower))));
		void MultiAttacks(ref PlayersAndProvinces pap, PInfo[] info, int[] my)
		{
			foreach (int to in AttackPlaces(pap, my).Distinct())
			{
				var starts = NeighborAllies(pap, to);
				int total = AttackersFrom(pap, info, starts, to).AttackPower;
				if (total > info[to].DefensePower + info[to].Enemies)
				{
					foreach (var from in starts)
					{
						Attack(ref pap, info, from, pap.Province(to), from.Soldiers.MaxAttackers(pap, from.Id, to).AttackedBy(Unit.Multiply(info[from.Id].Enemies - info[to].AttackPower)));
					}
				}
			}
		}
		void Transport(ref PlayersAndProvinces pap, PInfo[] info, Province from, Province to, Soldiers soldiers)
		{
			(pap, _) = pap.Do(new Move(this, from, to, new Army(soldiers, this)));
			info[from.Id].Soldiers -= soldiers;
			info[to.Id].Coming += soldiers.DefensePower;
		}
		void SpreadSoldiers(ref PlayersAndProvinces pap, PInfo[] info, int[] my)
		{
			foreach (int from in my)
			{
				foreach (var dest in pap.NeighborsOf(pap.Province(from)).Where(n => n.IsAllyOf(this)).OrderBy(n => info[n.Id].Bilance))
				{
					if (info[from].Enemies <= 0 && info[dest.Id].Enemies > 0)
					{
						Transport(ref pap, info, pap.Province(from), dest, info[from].Soldiers);
					}
					else if (info[from].Bilance > 0 && info[from].Bilance >= info[dest.Id].Bilance && info[from].Soldiers.DefensePower >= pap.Province(from).DefaultSoldiers.DefensePower)
					{
						Transport(ref pap, info, pap.Province(from), dest, info[from].Soldiers.AttackedBy(pap.Province(from).DefaultSoldiers));
					}
				}
			}
		}
		public PlayersAndProvinces Think(PlayersAndProvinces pap)
		{
			var my = pap.Provinces.Where(p => p.IsAllyOf(this)).Select(p => p.Id).ToArray();
			var info = pap.Provinces.Select(prov => new PInfo(prov.Soldiers, prov.IsAllyOf(this), prov.Occupied && !prov.IsAllyOf(this), EnemiesCount(pap, prov), 0)).ToArray();

			Recruits(ref pap, info, my.Select(i => pap.Provinces[i]).ToArray());
			Attacks(ref pap, info, my);
			MultiAttacks(ref pap, info, my);
			SpreadSoldiers(ref pap, info, my);
			return pap;
		}
	}
}
