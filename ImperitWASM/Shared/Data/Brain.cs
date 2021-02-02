using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public class Brain
	{
		readonly Player Player;
		readonly Settings Settings;
		public Brain(Player player, Settings settings)
		{
			Player = player;
			Settings = settings;
		}
		static int Max(params int[] values) => values.Max();
		static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;
		static int Updiv(int a, int b) => (a + b - 1) / b;

		int[] ComputeEnemies(Provinces provinces)
		{
			return provinces.Select(province => provinces.NeighborsOf(province).Where(n => n.IsEnemyOf(Player.Id)).Sum(n => n.AttackPower)).ToArray();
		}
		int[] ComputeAllies(Provinces provinces, int[] enemies, int[] defense)
		{
			return provinces.Select(province => provinces.NeighborIndices(province).Where(n => provinces[n].IsAllyOf(Player.Id)).Sum(n => Clamp(provinces[n].DefensePower - enemies[n] + defense[n], 0, provinces[n].DefensePower))).ToArray();
		}
		IEnumerable<int> EnemyNeighborIndices(Provinces provinces, int i)
		{
			return provinces.NeighborIndices(provinces[i]).Where(n => provinces[n].IsEnemyOf(Player.Id));
		}
		IEnumerable<int> AllyNeighborIndices(Provinces provinces, int i)
		{
			return provinces.NeighborIndices(provinces[i]).Where(n => provinces[n].IsAllyOf(Player.Id));
		}
		int[] Attackable(Provinces provinces, IEnumerable<int> owned)
		{
			return owned.SelectMany(i => EnemyNeighborIndices(provinces, i)).Distinct().ToArray();
		}

		SoldierType BestDefender(int i) => Settings.SoldierTypes.Where(type => type.IsRecruitable(Settings.Regions[i])).OrderBy(type => Player.Money / type.Price * type.DefensePower).First();
		Ship? BestShip(int i) => Settings.SoldierTypes.OfType<Ship>().Where(type => type.IsRecruitable(Settings.Regions[i]) && type.Price <= Player.Money).OrderByDescending(type => type.Capacity).FirstOrDefault();

		int EnemiesAfterAttack(Provinces provinces, int[] enemies, int[] defense, int start, int attacked)
		{
			return Max(0, enemies[start] - defense[start] + (provinces[attacked].IsEnemyOf(Player.Id) ? enemies[attacked] : 0));
		}
		Soldiers AttackingSoldiers(Provinces provinces, int[] enemies, int[] defense, int start, int attacked)
		{
			var movable = provinces[start].MaxMovable(provinces, provinces[attacked]);
			return movable.FightAgainst(EnemiesAfterAttack(provinces, enemies, defense, start, attacked), type => type.DefensePower);
		}

		static (Player, Provinces) Thinking(Brain ai, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			(Brain, Provinces) Do(Brain doer, ICommand command)
			{
				var (new_players, new_provinces) = command.Perform(doer.Player, players, provinces, settings);
				return (new Brain(new_players.First(p => p == doer.Player), settings), provinces.With(new_provinces))!;
			}
			int[] enemies = ai.ComputeEnemies(provinces);
			int[] defense = provinces.Select(province => province.DefensePower).ToArray();
			int[] allies = ai.ComputeAllies(provinces, enemies, defense); // Neighbor provinces which can send reinforcements
			int[] owned = provinces.Indices(province => province.IsAllyOf(ai.Player.Id)).ToArray();

			// Defensive reinforcements from the safe provinces to the endangered
			for (int i = 0; i < owned.Length; ++i)
			{
				if (enemies[owned[i]] > defense[owned[i]] && allies[owned[i]] > 0)
				{
					foreach (int supporter in ai.AllyNeighborIndices(provinces, owned[i]))
					{
						if (enemies[supporter] < defense[supporter])
						{
							// First, I take all soldiers who can move, but supporter province should not be endangered,
							// therefore I subtract soldiers whose defense power equals to the power of potential enemies
							var moving = provinces[supporter].MaxMovable(provinces, provinces[i]);
							moving = moving.FightAgainst(enemies[supporter], type => type.DefensePower);

							(ai, provinces) = Do(ai, new Move(provinces[supporter], provinces[i], moving));
							defense[i] += moving.DefensePower;
							allies[i] -= moving.DefensePower;
						}
					}
				}
			}

			// Defensive recruitment, if attack can be stopped
			for (int i = 0; i < owned.Length && ai.Player.Money > 0; ++i)
			{
				var type = ai.BestDefender(i);
				if (enemies[i] > defense[i] && ai.Player.Money / type.Price * type.DefensePower >= enemies[i] - defense[i])
				{
					var recruited = new Soldiers(type, Updiv(enemies[i] - defense[i], type.DefensePower));
					(ai, provinces) = Do(ai, new Recruit(provinces[i], recruited));
					defense[i] += recruited.DefensePower;
				}
			}

			// Recruitments reducing the probability of a revolution
			for (int i = 0; i < owned.Length && ai.Player.Money > 0; ++i)
			{
				var type = ai.BestDefender(i);
				if (provinces[i].DefaultDefensePower > defense[i] && ai.Player.Money >= type.Price)
				{
					var recruited = new Soldiers(type, Clamp((provinces[i].DefaultDefensePower - defense[i]) / type.DefensePower, 0, ai.Player.Money / type.Price));
					(ai, provinces) = Do(ai, new Recruit(provinces[i], recruited));
					defense[i] += recruited.DefensePower;
				}
			}

			// Recruitments of ships
			for (int i = 0; i < owned.Length && ai.Player.Money > 0; ++i)
			{
				if (ai.BestShip(i) is Ship ship)
				{
					(ai, provinces) = Do(ai, new Recruit(provinces[i], new Soldiers(ship, 1)));
					defense[i] += ship.DefensePower;
				}
			}

			// Attacks
			foreach (int attacked in ai.Attackable(provinces, owned))
			{
				int[] starts = ai.AllyNeighborIndices(provinces, attacked).ToArray();
				var armies = starts.Select(i => ai.AttackingSoldiers(provinces, enemies, defense, i, attacked)).ToArray();
				if (armies.Sum(soldiers => soldiers.AttackPower) > defense[attacked])
				{
					if (provinces[attacked].IsEnemyOf(ai.Player.Id))
					{
						foreach (int neighbour in provinces.NeighborIndices(provinces[attacked]))
						{
							enemies[neighbour] -= provinces[attacked].AttackPower;
						}
					}
					for (int i = 0; i < starts.Length; ++i)
					{
						(ai, provinces) = Do(ai, new Move(provinces[starts[i]], provinces[attacked], armies[i]));
					}
				}
			}
			return (ai.Player, provinces);
		}
		public (Player, Provinces) Think(IReadOnlyList<Player> players, Provinces provinces)
		{
			return Thinking(this, players, provinces, Settings);
		}
	}
}
