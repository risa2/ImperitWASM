using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record GiveUp : ICommand
	{
		readonly NextTurn next;
		public GiveUp() => next = new NextTurn();
		public bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings) => actor.Alive;
		public (IEnumerable<Player>, IEnumerable<Province>, Game, IEnumerable<Powers>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			players = players.Select(altered => actor == altered ? altered.Die() : altered).ToImmutableArray();
			provinces = provinces.With(provinces.Select(altered => altered.IsAllyOf(actor.Id) ? altered.Revolt() : altered));
			return actor.Active ? next.Perform(actor, players, provinces, settings, game) : (players, provinces, game, Enumerable.Empty<Powers>());
		}
	}
}
