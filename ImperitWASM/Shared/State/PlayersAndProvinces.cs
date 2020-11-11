using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class PlayersAndProvinces
	{
		public readonly ImmutableArray<Player> Players;
		public readonly Provinces Provinces;
		public PlayersAndProvinces(ImmutableArray<Player> players, Provinces provinces)
		{
			Players = players;
			Provinces = provinces;
		}
		public bool Passable(Province from, Province to, int distance, Func<Province, Province, int> difficulty) => Provinces.Passable(from, to, distance, difficulty);
		public int NeighborCount(Province prov) => Provinces.NeighborCount(prov);
		public IEnumerable<Province> NeighborsOf(Province prov) => Provinces.NeighborsOf(prov);
		public int IncomeOf(Player player) => Provinces.ControlledBy(player).OfType<Land>().Sum(p => p.Earnings);
		public bool HasAny(Player player) => Provinces.ControlledBy(player).Any();
		public Player? Winner(int finalCount) => Players.FirstOrDefault(p => Provinces.ControlledBy(p).OfType<Land>().Count(l => l.IsFinal) >= finalCount);
		public int PlayersCount => Players.Length;
		public int ProvincesCount => Provinces.Count;
		public Player Player(int i) => Players[i];
		public Province Province(int i) => Provinces[i];
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
		public (PlayersAndProvinces, bool) Do(ICommand cmd)
		{
			if (cmd.Allowed(this))
			{
				var new_players = Players.Select(player => cmd.Perform(player, this)).ToImmutableArray();
				var new_provinces = Provinces.With(Provinces.Select(province => cmd.Perform(province)).ToImmutableArray());
				return (new PlayersAndProvinces(new_players, new_provinces), true);
			}
			return (new PlayersAndProvinces(Players, Provinces), false);
		}
		public PlayersAndProvinces RemovePlayers(Player init)
		{
			var new_provinces = Provinces.With(Provinces.Select(p => p.Revolt()).ToImmutableArray());
			return new PlayersAndProvinces(ImmutableArray.Create(init), new_provinces);
		}
		public PlayersAndProvinces Add(IEnumerable<(Player, Soldiers, int)> starts)
		{
			var new_provinces = Provinces.ToBuilder();
			foreach (var (player, soldiers, where) in starts)
			{
				new_provinces[where] = new_provinces[where].GiveUpTo(player, soldiers);
			}
			return new PlayersAndProvinces(Players.AddRange(starts.Select(s => s.Item1)), Provinces.With(new_provinces.MoveToImmutable()));
		}
		public PlayersAndProvinces Add(Player p) => new PlayersAndProvinces(Players.Add(p), Provinces);
		public int LivingHumans => Players.Count(p => p.Alive && p is Human);
		public IEnumerable<(Player Player, IEnumerable<Province> Provinces)> PlayersProvinces => Players.Select(p => (p, Provinces.ControlledBy(p)));
	}
}
