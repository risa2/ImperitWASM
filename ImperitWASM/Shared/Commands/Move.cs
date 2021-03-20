using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record Move(Province From, Province To, Soldiers Soldiers) : ICommand
	{
		public bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return actor.Active && From.CanMove(provinces, To, actor.Id, Soldiers);
		}
		public (IEnumerable<Player>, IEnumerable<Province>, Game, IEnumerable<Powers>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			return (players.Select(altered => altered == actor ? altered.Add(new Manoeuvre(To.Order, Soldiers)) : altered), provinces.Select(altered => altered == From ? altered.Subtract(Soldiers) : altered), game, Enumerable.Empty<Powers>());
		}
		public bool HasEnoughCapacity(Provinces provinces) => Soldiers.Capacity(provinces, From, To) >= 0;
	}
}