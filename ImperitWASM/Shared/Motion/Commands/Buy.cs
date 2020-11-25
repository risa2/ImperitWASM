using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public record Buy(Player Player, Province Province, int Price) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap)
			=> Player.Money >= Price && pap.NeighborsOf(Province).Any(prov => prov is Land land && land.IsAllyOf(Player));
		public Province Perform(Province province)
		{
			return province == Province ? province.GiveUpTo(Player) : province;
		}
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return Player == player ? player.ChangeMoney(-Price) : player;
		}
	}
}