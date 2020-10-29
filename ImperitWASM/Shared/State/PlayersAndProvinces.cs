using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class PlayersAndProvinces
	{
		public readonly ImmutableArray<Player> Players;
		public readonly Provinces Provinces;
		readonly int active;
		public PlayersAndProvinces(ImmutableArray<Player> players, Provinces provinces, int active)
		{
			Players = players;
			Provinces = provinces;
			this.active = active;
		}
		public bool Passable(int from, int to, int distance, Func<Province, Province, int> difficulty) => Provinces.Passable(from, to, distance, difficulty);
		public int NeighborCount(Province prov) => Provinces.NeighborCount(prov.Id);
		public IEnumerable<Province> NeighborsOf(Province prov) => Provinces.NeighborsOf(prov.Id);
		public int IncomeOf(Player player) => Provinces.ControlledBy(player).Sum(p => p.Earnings);
		public bool HasAny(Player player) => Provinces.ControlledBy(player).Any();
		public int? Victory(int finalCount) => Players.Find(p => p is Human && Provinces.ControlledBy(p).OfType<Land>().Count(l => l.IsFinal) >= finalCount);
		public int PlayersCount => Players.Length;
		public int ProvincesCount => Provinces.Count;
		public Player Player(int i) => Players[i];
		public Province Province(int i) => Provinces[i];
		public PlayersAndProvinces Act(bool includePlayerActions = true)
		{
			var new_players = Players.ToBuilder();
			var new_provinces = Provinces.ToBuilder();
			for (int i = 0; i < Provinces.Count; ++i)
			{
				var player = new_players[active];
				(new_provinces[i], new_players) = new_provinces[i].Act(new PlayersAndProvinces(new_players.MoveToImmutable(), Provinces, active), player);
			}
			if (includePlayerActions)
			{
				(new_provinces, new_players[active]) = new_players[active].Act(new PlayersAndProvinces(new_players.ToImmutable(), Provinces.With(new_provinces.MoveToImmutable()), active));
			}
			return new PlayersAndProvinces(new_players.MoveToImmutable(), Provinces.With(new_provinces.MoveToImmutable()), active);
		}
		public (PlayersAndProvinces, bool) Do(ICommand cmd)
		{
			if (cmd.Allowed(this))
			{
				var new_players = Players.Select(player => cmd.Perform(player, this)).ToImmutableArray();
				var new_provinces = Provinces.With(Provinces.Select(province => cmd.Perform(province)).ToImmutableArray());
				return (new PlayersAndProvinces(new_players, new_provinces, active), true);
			}
			return (new PlayersAndProvinces(Players, Provinces, active), false);
		}
		public PlayersAndProvinces RemovePlayers()
		{
			var new_provinces = Provinces.With(Provinces.Select(p => p.Revolt()).ToImmutableArray());
			return new PlayersAndProvinces(ImmutableArray<Player>.Empty, new_provinces, 0);
		}
		public PlayersAndProvinces Add(IEnumerable<(Player, Soldiers, int)> starts)
		{
			var new_provinces = Provinces.ToBuilder();
			foreach (var (player, soldiers, where) in starts)
			{
				new_provinces[where] = new_provinces[where].GiveUpTo(new Army(soldiers, player));
			}
			return new PlayersAndProvinces(Players.AddRange(starts.Select(s => s.Item1)), Provinces.With(new_provinces.MoveToImmutable()), active);
		}
		public PlayersAndProvinces Add(Player p) => new PlayersAndProvinces(Players.Add(p), Provinces, active);
		public PlayersAndProvinces Next() => new PlayersAndProvinces(Players, Provinces, Players.FirstRotated(active + 1, p => p.Alive && !(p is Savage), 0));
		public PlayersAndProvinces ResetActive() => new PlayersAndProvinces(Players, Provinces, Players.FirstRotated(0, p => p.Alive && !(p is Savage), 0));
		public int LivingHumans => Players.Count(p => p.Alive && p is Human);
		public Player Active => Players[active];
		public override string ToString() => active.ToString(CultureInfo.InvariantCulture);
		public IEnumerable<(Player Player, IEnumerable<Province> Provinces)> PlayersProvinces => Players.Select(p => (p, Provinces.ControlledBy(p)));
	}
}
