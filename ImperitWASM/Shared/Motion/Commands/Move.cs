using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Motion.Commands
{
	public record Move(Player Player, Province From, Province To, Soldiers Soldiers) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap)
		{
			return From.IsAllyOf(Player) && Soldiers.CanMove(pap, From, To) && To.Subtract(Soldiers).CanSoldiersSurvive;
		}
		public Province Perform(Province province)
		{
			return province == From ? province.Subtract(Soldiers) : province == To ? province.Add(new Manoeuvre(Player, Soldiers)) : province;
		}
	}
}