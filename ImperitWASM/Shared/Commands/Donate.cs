using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record Donate(Player Recipient, int Amount) : ICommand
	{
		public bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return actor.Money >= Amount && Amount > 0 && actor != Recipient;
		}
		public (IEnumerable<Player>, IEnumerable<Province>, Game) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			return (players.Select(altered => altered == Recipient ? altered.ChangeMoney(Amount) : altered == actor ? altered.ChangeMoney(-Amount) : altered), provinces, game);
		}
	}
}