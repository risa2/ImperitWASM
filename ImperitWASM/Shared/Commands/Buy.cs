using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Commands
{
	public record Buy(Player Player, Province Province, int Price) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap) => Player.Money >= Price && pap.HasNeighborRuledBy(Province, Player);
		public Province Perform(Province province) => Province == province ? province.GiveUpTo(Player) : province;
		public Player Perform(Player player, PlayersAndProvinces pap) => Player == player ? player.ChangeMoney(-Price) : player;
	}
}