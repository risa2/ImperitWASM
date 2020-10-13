using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Attack : Move
	{
		public Attack(Player player, Province from, Province to, Army army) : base(player, from, to, army) { }
		protected override IAction Action => new Fight(Army);
		public override bool Allowed(PlayersAndProvinces pap) => Army.AttackPower > 0 && base.Allowed(pap);
	}
}