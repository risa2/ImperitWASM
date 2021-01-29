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
			return actor.Active && From.CanMove(provinces, To, actor, Soldiers);
		}
		public (IEnumerable<Player>, IEnumerable<Province>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return (players.Select(altered => altered == actor ? altered.Add(new Manoeuvre(To.Order, Soldiers)) : altered), provinces.Select(altered => altered == From ? altered.Subtract(Soldiers) : altered));
		}
		public bool HasEnoughCapacity(Provinces provinces) => Soldiers.Capacity(provinces, From, To) >= 0;
	}
}