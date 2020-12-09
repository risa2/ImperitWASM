using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Commands;

namespace ImperitWASM.Shared.State
{
	public record PlayersAndProvinces(ImmutableArray<Player> Players, Provinces Provinces)
	{
		public IEnumerable<Land> Lands => Provinces.OfType<Land>();
		public bool Passable(Province from, Province to, int distance, Func<Province, Province, int> difficulty) => Provinces.Passable(from, to, distance, difficulty);
		public int NeighborCount(Province prov) => Provinces.NeighborCount(prov);
		public IEnumerable<Province> NeighborsOf(Province prov) => Provinces.NeighborsOf(prov);
		public IEnumerable<int> NeighborIndices(Province prov) => Provinces.NeighborIndices(prov);
		public int IncomeOf(Player player) => Provinces.ControlledBy(player).OfType<Land>().Sum(p => p.Earnings);
		public bool HasAny(Player player) => Provinces.ControlledBy(player).Any();
		public bool HasNeighborRuledBy(Province province, Player player) => NeighborsOf(province).Any(prov => prov is Land land && land.IsAllyOf(player));

		public Player? Winner(int finalCount) => Players.FirstOrDefault(p => Provinces.ControlledBy(p).OfType<Land>().Count(l => l.IsFinal) >= finalCount);
		public int PlayersCount => Players.Length;
		public int ProvincesCount => Provinces.Count;
		public Player Player(int i) => Players[i];
		public Province Province(int i) => Provinces[i];
		public int Next(int active) => (Players.Indices(p => p.Alive && p is not Savage).Min(i => (i - active - 1 + PlayersCount) % PlayersCount) + active + 1) % PlayersCount;

		public PlayersAndProvinces Act(int active, bool includePlayerActions = true)
		{
			var new_players = Players.ToBuilder();
			var new_provinces = Provinces.ToBuilder();
			for (int i = 0; i < Provinces.Count; ++i)
			{
				(new_provinces[i], new_players) = new_provinces[i].Act(new PlayersAndProvinces(new_players.MoveToImmutable(), Provinces));
			}
			if (includePlayerActions)
			{
				(new_provinces, new_players[active]) = new_players[active].Act(new PlayersAndProvinces(new_players.ToImmutable(), Provinces.With(new_provinces.MoveToImmutable())));
			}
			return new PlayersAndProvinces(new_players.MoveToImmutable(), Provinces.With(new_provinces.MoveToImmutable()));
		}
		public (PlayersAndProvinces, bool) Add(ICommand cmd)
		{
			if (cmd.Allowed(this))
			{
				var new_players = Players.Select(player => cmd.Perform(player, this)).ToImmutableArray();
				var new_provinces = Provinces.With(Provinces.Select(province => cmd.Perform(province)).ToImmutableArray());
				return (new PlayersAndProvinces(new_players, new_provinces), true);
			}
			return (this, false);
		}
		public PlayersAndProvinces JustAdd(ICommand cmd) => Add(cmd).Item1;
		public int LivingHumans => Players.Count(p => p.Alive && p is Human);
		public PlayersPower PlayersPower(Func<Player, bool> which) => new PlayersPower(Players.Where(which).Select(p => p.Power(Provinces.ControlledBy(p).ToImmutableArray())).ToImmutableArray());
		public IEnumerable<int> Inhabitable => Provinces.Indices(it => it is Land { IsInhabitable: true });
		public PlayersAndProvinces AddRobots(Settings settings, Func<string, int, string> mod)
		{
			var new_players = Players.ToBuilder();
			var new_provinces = Provinces.ToBuilder();
			foreach (int start in Inhabitable)
			{
				new_players.Add(Robot.Create(Settings.ColorOf(new_players.Count), settings.GetName(new_players.Count - Players.Length, mod), settings.StartMoney(start), settings));
				new_provinces[start] = new_provinces[start].GiveUpTo(new_players[^1], new_provinces[start].Soldiers);
			}
			return new PlayersAndProvinces(new_players.ToImmutable(), new Provinces(new_provinces.MoveToImmutable(), settings));
		}

	}
}
