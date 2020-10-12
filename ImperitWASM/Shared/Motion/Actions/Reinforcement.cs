using ImperitWASM.Shared.Motion.Commands;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Actions
{
	public class Reinforcement : Movement
	{
		public Reinforcement(int province, Army army) : base(province, army) { }
		public override (IAction?, Province) Perform(Province province, Player active)
		{
			return Province == province.Id ? (null, province.ReinforcedBy(Soldiers)) : (this, province);
		}
		public override (IAction, bool) Interact(ICommand another) => another switch
		{
			Reinforce reinf when Army.IsAllyOf(reinf.Player) && reinf.To == Province
				=> (new Reinforcement(Province, Army.Join(reinf.Soldiers)), false),
			Recruit recr when Army.IsAllyOf(recr.Player) && recr.Land == Province
				=> (new Reinforcement(Province, Army.Join(recr.Soldiers)), false),
			_ => (this, true)
		};
	}
}