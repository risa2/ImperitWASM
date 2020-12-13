using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record Recruit(Player Player, Province Province, Soldiers Soldiers) : ICommand
	{
		public Player Perform(Player player, PlayersAndProvinces pap)
		{
			return player == Player ? player.ChangeMoney(-Soldiers.Price) : player;
		}
		public Province Perform(Province province)
		{
			return province == Province ? province.Add(new Manoeuvre(Player, Soldiers)) : province;
		}
		public bool Allowed(PlayersAndProvinces pap)
		{
			return Province.IsAllyOf(Player) && Player.Money >= Soldiers.Price && Soldiers.Any;
		}
	}
}