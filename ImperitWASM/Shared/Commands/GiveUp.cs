using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record GiveUp : NextTurn
	{
		public override bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings) => true;
		public override (IEnumerable<Player>, IEnumerable<Province>, Game) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			var (new_players, new_provinces, new_game) = actor.Active ? base.Perform(actor, players, provinces, settings, game) : (players, provinces, game);
			return (new_players.Select(altered => actor == altered ? altered.Die() : altered), new_provinces.Select(altered => altered.IsAllyOf(actor.Id) ? altered.Revolt() : altered), game);
		}
	}
}
