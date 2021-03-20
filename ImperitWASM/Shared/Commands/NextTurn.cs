using System.Collections.Generic;
using System.Collections.Immutable;
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
		static (IEnumerable<Player>, IEnumerable<Province>) Clear(PlayerIdentity actor_id, IEnumerable<Player> players, IEnumerable<Province> provinces)
		{
			var cleared_players = players.Select(altered => provinces.Any(province => province.IsAllyOf(altered.Id) && (province.HasSoldiers || province.Mainland)) ? altered : altered.Die()).ToArray();
			var cleared_provinces = provinces.Select(altered => cleared_players.Any(player => player.Alive || !altered.IsAllyOf(player.Id)) ? altered.RevoltIfShaky(actor_id) : altered.Revolt());
			return (cleared_players, cleared_provinces);
		}
		static Player[] NextActive(int active, IReadOnlyList<Player> players)
		{
			int next_active = players.Indices(p => p.Alive).OrderBy(i => (i - active + players.Count - 1) % players.Count).First();
			return players.Select((altered, i) => i == next_active ? altered.InvertActive : altered).ToArray();
		}
		static (IReadOnlyList<Player>, Provinces) EndOfTurn(IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			int active = players.Indices(p => p.Active).First();
			var (new_players, new_provinces) = Act(players[active], players, provinces, settings);
			(new_players, new_provinces) = Clear(players[active].Id, new_players, new_provinces.ToArray());
			return (NextActive(active, new_players.ToArray()), provinces.With(new_provinces));
		}

		public virtual (IEnumerable<Player>, IEnumerable<Province>, Game, IEnumerable<Powers>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			var (new_players, new_provinces) = EndOfTurn(players, provinces, settings);
			var powers = new List<Powers>(1);
			while (new_players.First(p => p.Active) is { Human: false } robot && new_players.Count(p => p.LivingHuman) > 1)
			{
				(new_players, new_provinces) = robot.Think(new_players, new_provinces, settings, game);
				(new_players, new_provinces) = EndOfTurn(new_players, new_provinces, settings);

				powers.Add(new Powers(new_players.Select(p => p.Power(new_provinces)).ToImmutableArray()));
			}
			bool finish = new_players.Count(p => p.LivingHuman) < 2 || new_provinces.Winner.Item2 >= settings.FinalLandsCount;
			return (new_players, new_provinces, finish ? game.Finish(): game, powers);
		}
	}
}
