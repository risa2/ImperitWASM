using ImperitWASM.Shared.State;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Buy : ICommand
	{
		public readonly Player Player;
		public readonly int Land;
		public readonly int Price;
		public Buy(Player player, int land, int price)
		{
			Player = player;
			Land = land;
			Price = price;
		}
		public bool Allowed(IReadOnlyList<Player> players, IProvinces provinces)
			=> players[Player.Id].Money >= Price && provinces.NeighborsOf(Land).Any(prov => prov is Land land && land.IsAllyOf(Player.Id));

		public (IAction?, Province) Perform(Province province)
		{
			return (null, province.Id == Land ? province.GiveUpTo(Player) : province);
		}
		public (IAction?, Player) Perform(Player player, IProvinces provinces)
		{
			return (null, Player == player ? player.ChangeMoney(-Price) : player);
		}
	}
}