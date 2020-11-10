using ImperitWASM.Shared.Cfg;
using ImperitWASM.Shared.Entities;
using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Cmd
{
	public class NextTurn : ICommand
	{
		public Player Perform(Settings settings, Player player, PlayersAndProvinces pap) => pap.HasAny(player) ? player : player.Die();
	}
}
