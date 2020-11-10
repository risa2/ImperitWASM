using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public class Recruit : ICommand
	{
		public readonly Player Player;
		public readonly Province Province;
		public readonly Soldiers Soldiers;
		public Recruit(Player player, Province province, Soldiers soldeirs)
		{
			Player = player;
			Province = province;
			Soldiers = soldeirs;
		}
		public Player Perform(Settings settings, Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(-Soldiers.Price) : player;
		}
		public Province Perform(Settings settings, Province province)
		{
			return province == Province ? province.Add(new Manoeuvre(Player, Soldiers)) : province;
		}
		public bool Allowed(Settings settings, PlayersAndProvinces pap)
		{
			return Province.IsAllyOf(Player) && Player.Money >= Soldiers.Price && Soldiers.Any;
		}
	}
}