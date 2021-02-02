using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record NextTurn : ICommand
	{
		public virtual bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return actor.Active;
		}
		static (IEnumerable<Player>, IEnumerable<Province>) Act(Player actor, IEnumerable<Player> players, Provinces provinces, Settings settings)
		{
			(actor, provinces) = actor.Earn(provinces).Act(provinces, settings);
			return (players.Select(altered => altered == actor ? actor.InvertActive : altered), provinces);
		}
		static (IEnumerable<Player>, IEnumerable<Province>) Clear(Player actor, IEnumerable<Player> players, IEnumerable<Province> provinces)
		{
			var cleared_players = players.Select(altered => provinces.Any(province => province.IsAllyOf(altered.Id) && (province.HasSoldiers || province.Mainland)) ? altered : altered.Die()).ToArray();
			var cleared_provinces = provinces.Select(altered => cleared_players.Any(player => player.Alive && altered.IsAllyOf(player.Id)) ? altered.RevoltIfShaky(actor.Id) : altered.Revolt());
			return (cleared_players, cleared_provinces);
		}
		static Player[] NextActive(int active, IReadOnlyList<Player> players)
		{
			int next_active = players.Indices(p => p.Alive).OrderBy(i => (i - active + players.Count - 1) % players.Count).First();
			return players.Select((altered, i) => i == next_active ? altered.InvertActive : altered).ToArray();
		}
		static (Player[], Provinces) EndOfTurn(Player actor, IEnumerable<Player> players, Provinces provinces, Settings settings)
		{
			int active = players.Indices(p => p.Active).First();
			var (new_players, new_provinces) = Act(actor, players, provinces, settings);
			(new_players, new_provinces) = Clear(actor, new_players, new_provinces.ToArray());
			return (NextActive(active, new_players.ToArray()), provinces.With(new_provinces));
		}
		public virtual (IEnumerable<Player>, IEnumerable<Province>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			var (new_players, new_provinces) = EndOfTurn(actor, players, provinces, settings);
			while (new_players.First(p => p.Active) is { Human: false } robot && new_players.Count(p => p is { LivingHuman: true }) > 1)
			{
				(robot, new_provinces) = new Brain(robot, settings).Think(new_players, new_provinces);
				(new_players, new_provinces) = EndOfTurn(robot, new_players.Select(player => player == robot ? robot : player).ToArray(), new_provinces, settings);
			}
			return (new_players, new_provinces);
		}
	}
}
