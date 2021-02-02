using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record GiveUp : NextTurn
	{
		public override bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings) => true;
		public override (IEnumerable<Player>, IEnumerable<Province>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			var (new_players, new_provinces) = actor.Active ? base.Perform(actor, players, provinces, settings) : (players, provinces);
			return (new_players.Select(altered => actor == altered ? altered.Die() : altered), new_provinces.Select(altered => altered.IsAllyOf(actor.Id) ? altered.Revolt() : altered));
		}
	}
}
