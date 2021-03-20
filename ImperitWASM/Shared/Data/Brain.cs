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
		IEnumerable<int> EnemyNeighborIndices(Provinces provinces, int i)
		{
			return provinces.NeighborIndices(provinces[i]).Where(n => provinces[n].IsEnemyOf(Player));
		}
		IEnumerable<int> AllyNeighborIndices(Provinces provinces, int i)
		{
			return provinces.NeighborIndices(provinces[i]).Where(n => provinces[n].IsAllyOf(Player));
		}
		int[] Attackable(Provinces provinces, IEnumerable<int> owned)
		{
			return owned.SelectMany(i => EnemyNeighborIndices(provinces, i)).Distinct().ToArray();
		}

		SoldierType? BestDefender(int i, int money) => Settings.RecruitableIn(i).OrderBy(type => money / type.Price * type.DefensePower).FirstOrDefault();
		Ship? BestShip(int i, int money) => Settings.RecruitableIn(i).OfType<Ship>().Where(type => type.Price <= money).OrderByDescending(type => type.Capacity).FirstOrDefault();

		int EnemiesAfterAttack(Provinces provinces, int[] enemies, int[] defense, int start, int attacked)
		{
			return Max(0, enemies[start] - defense[start] + (provinces[attacked].IsEnemyOf(Player) ? enemies[attacked] : 0));
		}
		Soldiers AttackingSoldiers(Provinces provinces, int[] enemies, int[] defense, int start, int attacked)
		{
			var movable = provinces[start].MaxMovable(provinces, provinces[attacked]);
			return movable.FightAgainst(EnemiesAfterAttack(provinces, enemies, defense, start, attacked), type => type.DefensePower);
		}
		(IReadOnlyList<Player>, Provinces) Do(ICommand command, int player, IReadOnlyList<Player> players, Provinces provinces)
		{
			var (new_players, new_provinces, _, _) = command.Perform(players[player], players, provinces, Settings, Game);
			return (new_players.ToImmutableArray(), provinces.With(new_provinces))!;
		}

		(IReadOnlyList<Player>, Provinces) Thinking(IReadOnlyList<Player> players, Provinces provinces)
		{
			int player = players.Indices(player => player.Id == Player).First();
			int[] enemies = ComputeEnemies(provinces);
			int[] defense = provinces.Select(province => province.DefensePower).ToArray();
			int[] allies = ComputeAllies(provinces, enemies, defense); // Neighbor provinces which can send reinforcements
			int[] owned = provinces.Indices(province => province.IsAllyOf(Player)).ToArray();

			// Defensive reinforcements from the safe provinces to the endangered
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

			// Recruitments of ships
			for (int i = 0; i < owned.Length && players[player].Money > 0; ++i)
			{
				if (BestShip(owned[i], players[player].Money) is Ship ship)
				{
					(players, provinces) = Do(new Recruit(provinces[owned[i]], new Soldiers(ship, 1)), player, players, provinces);
					defense[owned[i]] += ship.DefensePower;
				}
			}

			// Attacks
			foreach (int attacked in Attackable(provinces, owned))
			{
				int[] starts = AllyNeighborIndices(provinces, attacked).ToArray();
				var armies = starts.Select(i => AttackingSoldiers(provinces, enemies, defense, i, attacked)).ToArray();
				if (armies.Sum(soldiers => soldiers.AttackPower) > defense[attacked])
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
			return (players, provinces);
		}
		public static (IReadOnlyList<Player>, Provinces) Think(PlayerIdentity ip, IReadOnlyList<Player> players, Provinces provinces, Game game, Settings settings)
		{
			return new Brain(ip, settings, game).Thinking(players, provinces);
		}
	}
}
