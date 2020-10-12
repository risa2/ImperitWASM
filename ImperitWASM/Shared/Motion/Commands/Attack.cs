using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.State;
using System.Collections.Generic;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Attack : Move
	{
		public Attack(int player, int from, int to, Army army) : base(player, from, to, army) { }
		protected override Movement Action => new Fight(To, Army);
		public override bool Allowed(IReadOnlyList<Player> players, IProvinces provinces) => Army.AttackPower > 0 && base.Allowed(players, provinces);
	}
}