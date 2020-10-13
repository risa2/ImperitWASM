using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
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
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(-Soldiers.Price) : player;
		}
		public Province Perform(Province province)
		{
			return province == Province ? province.Add(new Actions.Reinforcement(Soldiers)) : province;
		}
		public bool Allowed(PlayersAndProvinces pap)
		{
			return Province.IsAllyOf(Player) && Player.Money >= Soldiers.Price && Soldiers.Any;
		}
	}
}