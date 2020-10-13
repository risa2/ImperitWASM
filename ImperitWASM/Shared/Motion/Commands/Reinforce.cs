using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Reinforce : Move
	{
		public Reinforce(Player player, Province from, Province to, Army army) : base(player, from, to, army) { }
		protected override IAction Action => new Reinforcement(Army.Soldiers);
	}
}