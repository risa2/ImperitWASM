using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record Buy(Player Player, Land Land) : ICommand
	{
		bool Possible(PlayersAndProvinces pap) => pap.HasNeighborRuledBy(Land, Player);
		public bool AllowedWithLoan(PlayersAndProvinces pap) => Player.MaxUsableMoney >= Land.Price && Possible(pap);
		public bool Allowed(PlayersAndProvinces pap) => Player.Money >= Land.Price && Possible(pap);
		public Province Perform(Province province) => Land == province ? province.ConqueredBy(Player) : province;
		public Player Perform(Player player, PlayersAndProvinces pap) => Player == player ? player.ChangeMoney(-Land.Price) : player;
	}
}