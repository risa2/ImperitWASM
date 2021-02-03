using System.Collections.Generic;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public sealed record Recruit(Province Province, Soldiers Soldiers) : ICommand
	{
		public bool Allowed(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings)
		{
			return actor.Active && Province.IsAllyOf(actor.Id) && actor.MaxUsableMoney >= Soldiers.Price && Soldiers.Any;
		}
		public (IEnumerable<Player>, IEnumerable<Province>, Game) Perform(Player actor, IReadOnlyList<Player> players, Provinces provinces, Settings settings, Game game)
		{
			return (players.Select(altered => altered.Active ? altered.Pay(Soldiers.Price).Add(new Manoeuvre(Province.Order, Soldiers)) : altered), provinces, game);
		}
	}
}