using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record GiveUp(Player Player) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap) => true;
		public Province Perform(Province province) => province.IsAllyOf(Player) ? province.Revolt() : province;
		public Player Perform(Player player, PlayersAndProvinces pap) => player == Player ? player.Die() : player;
	}
}
