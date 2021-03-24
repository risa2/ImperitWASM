using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public sealed record Brain(PlayerIdentity Player, Settings Settings, Game Game)
	{
		static int Max(params int[] values) => values.Max();
		static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;
		static int Updiv(int a, int b) => (a + b - 1) / b;

		int[] ComputeEnemies(Provinces provinces)
		{
			return provinces.Select(province => provinces.NeighborsOf(province).Where(n => n.IsEnemyOf(Player)).Sum(n => n.AttackPower)).ToArray();
		}
		int[] ComputeAllies(Provinces provinces, int[] enemies, int[] defense)
		{
			return provinces.Select(province => provinces.NeighborIndices(province).Where(n => provinces[n].IsAllyOf(Player)).Sum(n => Clamp(provinces[n].DefensePower - enemies[n] + defense[n], 0, provinces[n].DefensePower))).ToArray();
		}
		IEnumerable<int> AllyNeighborIndices(Provinces provinces, int i)
		{
			return provinces.NeighborIndices(provinces[i]).Where(n => provinces[n].IsAllyOf(Player));
		}
		static IEnumerable<int> ShiplessPorts(Provinces provinces, int[] owned)
		{
			return owned.Where(n => provinces[n].Port && !provinces[n].HasShip);
		}

		SoldierType? BestDefender(int i, int money) => Settings.RecruitableIn(i).OrderByDescending(type => money / type.Price * type.DefensePower).FirstOrDefault();
		SoldierType? BestAttacker(int i, int money) => Settings.RecruitableIn(i).OrderByDescending(type => money / type.Price * type.AttackPower).FirstOrDefault();
		Ship? BestShip(int i, int money) => Settings.RecruitableIn(i).OfType<Ship>().Where(type => type.Price <= money).OrderByDescending(type => type.Capacity).FirstOrDefault();

		(IReadOnlyList<Player>, Provinces) Do(ICommand command, int player, IReadOnlyList<Player> players, Provinces provinces)
		{
			var (new_players, new_provinces, _, _) = command.Perform(players[player], players, provinces, Settings, Game);
			return (new_players.ToImmutableArray(), provinces.With(new_provinces))!;
		}

		void ReinforcementsFromSafeToEndangeredProvinces(ref IReadOnlyList<Player> players, ref Provinces provinces, int player, int[] enemies, int[] defense, int[] allies, int[] owned)
		{
			for (int i = 0; i < owned.Length; ++i)
			{
				if (enemies[owned[i]] > defense[owned[i]] && allies[owned[i]] > 0)
				{
					foreach (int supporter in AllyNeighborIndices(provinces, owned[i]))
					{
						if (enemies[supporter] < defense[supporter])
						{
							// First, I take all soldiers who can move, but supporter province should not be endangered,
							// therefore I subtract soldiers whose defense power equals to the power of potential enemies
							var moving = provinces[supporter].MaxMovable(provinces, provinces[owned[i]]);
							moving = moving.FightAgainst(enemies[supporter], type => type.DefensePower);

							(players, provinces) = Do(new Move(provinces[supporter], provinces[owned[i]], moving), player, players, provinces);
							defense[owned[i]] += moving.DefensePower;
							allies[owned[i]] -= moving.DefensePower;
						}
					}
				}
			}
		}

		void DefensiveRecruitments(ref IReadOnlyList<Player> players, ref Provinces provinces, int player, int[] enemies, int[] defense, int[] owned)
		{
			// Defensive recruitment, if attack can be stopped
			for (int i = 0; i < owned.Length && players[player].Money > 0; ++i)
			{
				var type = BestDefender(owned[i], players[player].Money);
				if (type is not null && enemies[owned[i]] > defense[owned[i]] && players[player].Money / type.Price * type.DefensePower >= enemies[owned[i]] - defense[owned[i]])
				{
					var recruited = new Soldiers(type, Updiv(enemies[owned[i]] - defense[owned[i]], type.DefensePower));
					(players, provinces) = Do(new Recruit(provinces[owned[i]], recruited), player, players, provinces);
					defense[owned[i]] += recruited.DefensePower;
				}
			}

			// Recruitments reducing the probability of a revolution
			for (int i = 0; i < owned.Length && players[player].Money > 0; ++i)
			{
				var type = BestDefender(owned[i], players[player].Money);
				if (type is not null && provinces[owned[i]].DefaultDefensePower > defense[owned[i]] && players[player].Money >= type.Price)
				{
					var recruited = new Soldiers(type, Clamp((provinces[owned[i]].DefaultDefensePower - defense[owned[i]]) / type.DefensePower, 0, players[player].Money / type.Price));
					(players, provinces) = Do(new Recruit(provinces[owned[i]], recruited), player, players, provinces);
					defense[owned[i]] += recruited.DefensePower;
				}
			}
		}

		void OffensiveRecruitments(ref IReadOnlyList<Player> players, ref Provinces provinces, int player, int[] enemies, int[] defense, int[] owned)
		{
			// Recruitments of ships
			var provinces_arg = provinces;
			int port = ShiplessPorts(provinces, owned).MaxBy(n => provinces_arg[n].AttackPower, -1);
			if (port != -1 && BestShip(port, players[player].Money) is Ship ship)
			{
				(players, provinces) = Do(new Recruit(provinces[port], new Soldiers(ship, 1)), player, players, provinces);
				defense[port] += ship.DefensePower;
			}

			int remaining_money = players[player].Money;
			var (province, attacker) = owned.Select(n => (n, BestAttacker(n, remaining_money))).Where(pair => pair.Item2 is not null).MaxBy(pair => enemies[pair.n]);
			if (attacker is not null)
			{
				var soldiers = new Soldiers(attacker, remaining_money / attacker.Price);
				(players, provinces) = Do(new Recruit(provinces[province], soldiers), player, players, provinces);
				defense[province] += soldiers.DefensePower;
			}
		}

		IEnumerable<int> AttackableNeighborIndices(Provinces provinces, int i)
		{
			return provinces.NeighborIndices(provinces[i]).Where(n => !provinces[n].IsAllyOf(Player));
		}
		IEnumerable<int> Attackable(Provinces provinces, IEnumerable<int> owned)
		{
			return owned.SelectMany(i => AttackableNeighborIndices(provinces, i)).Distinct();
		}
		int EnemiesAfterAttack(Provinces provinces, int[] enemies, int start, int attacked)
		{
			return Max(0, enemies[start] - (provinces[attacked].IsEnemyOf(Player) ? provinces[attacked].AttackPower : 0));
		}
		Soldiers AttackingSoldiers(Provinces provinces, int[] enemies, int start, int attacked)
		{
			var movable = provinces[start].MaxMovable(provinces, provinces[attacked]);
			return movable.FightAgainst(EnemiesAfterAttack(provinces, enemies, start, attacked), type => type.DefensePower);
		}
		Soldiers[] AttackingSoldiers(Provinces provinces, int[] enemies, int[] starts, int attacked)
		{
			return starts.Select(i => AttackingSoldiers(provinces, enemies, i, attacked)).ToArray();
		}
		void Attacks(ref IReadOnlyList<Player> players, ref Provinces provinces, int player, int[] enemies, int[] defense, int[] owned)
		{
			foreach (int attacked in Attackable(provinces, owned))
			{
				int[] starts = AllyNeighborIndices(provinces, attacked).ToArray();
				var armies = AttackingSoldiers(provinces, enemies, starts, attacked);
				if (armies.Sum(soldiers => soldiers.AttackPower) > defense[attacked] + (provinces[attacked].IsEnemyOf(Player) ? 0 : enemies[attacked]))
				{
					if (provinces[attacked].IsEnemyOf(Player))
					{
						foreach (int neighbour in provinces.NeighborIndices(provinces[attacked]))
						{
							enemies[neighbour] -= provinces[attacked].AttackPower;
						}
					}
					for (int i = 0; i < starts.Length; ++i)
					{
						(players, provinces) = Do(new Move(provinces[starts[i]], provinces[attacked], armies[i]), player, players, provinces);
					}
				}
			}
		}

		(IReadOnlyList<Player>, Provinces) Thinking(IReadOnlyList<Player> players, Provinces provinces)
		{
			int player = players.Indices(player => player.Id == Player).First();
			int[] enemies = ComputeEnemies(provinces);
			int[] defense = provinces.Select(province => province.DefensePower).ToArray();
			int[] allies = ComputeAllies(provinces, enemies, defense); // Neighbor provinces which can send reinforcements
			int[] owned = provinces.Indices(province => province.IsAllyOf(Player)).ToArray();

			ReinforcementsFromSafeToEndangeredProvinces(ref players, ref provinces, player, enemies, defense, allies, owned);
			DefensiveRecruitments(ref players, ref provinces, player, enemies, defense, owned);
			OffensiveRecruitments(ref players, ref provinces, player, enemies, defense, owned);
			Attacks(ref players, ref provinces, player, enemies, defense, owned);
			return (players, provinces);
		}

		public static (IReadOnlyList<Player>, Provinces) Think(PlayerIdentity ip, IReadOnlyList<Player> players, Provinces provinces, Game game, Settings settings)
		{
			return new Brain(ip, settings, game).Thinking(players, provinces);
		}
	}
}
