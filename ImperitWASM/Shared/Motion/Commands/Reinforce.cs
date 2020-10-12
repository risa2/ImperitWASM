using ImperitWASM.Shared.Motion.Actions;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public class Reinforce : Move
	{
		public Reinforce(int player, int from, int to, Army army) : base(player, from, to, army) { }
		protected override Movement Action => new Reinforcement(To, Army);
	}
}