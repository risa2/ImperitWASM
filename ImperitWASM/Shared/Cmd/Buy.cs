using System.Linq;
using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public class Buy : ICommand
	{
		public readonly Player Player;
		public readonly Province Province;
		public readonly int Price;
		public Buy(Player player, Province province, int price)
		{
			Player = player;
			Province = province;
			Price = price;
		}
		public bool Allowed(Settings settings, PlayersAndProvinces pap)
			=> Player.Money >= Price && pap.NeighborsOf(Province).Any(prov => prov is Land land && land.IsAllyOf(Player));
		public Province Perform(Settings settings, Province province)
		{
			return province == Province ? province.GiveUpTo(Player) : province;
		}
		public Player Perform(Settings settings, Player player, PlayersAndProvinces pap)
		{
			return Player == player ? player.ChangeMoney(-Price) : player;
		}
	}
}