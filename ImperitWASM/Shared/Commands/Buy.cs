using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record Buy(Province Province) : ICommand
	{
		public bool Allowed(Player actor, Provinces provinces)
		{
			return actor.MaxUsableMoney >= Province.Price && provinces.HasNeighborRuledBy(Province, actor.Id);
		}
		public bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return Allowed(actor, provinces);
		}
		public (IEnumerable<Player>, IEnumerable<Province>) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return (players.Select(altered => altered.Active ? altered.Pay(Province.Price) : altered), provinces.Select(altered => Province == altered ? altered.RuledBy(actor.Id) : altered));
		}
	}
}