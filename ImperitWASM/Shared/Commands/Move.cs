using ImperitWASM.Shared.Data;

namespace ImperitWASM.Shared.Commands
{
	public record Move(Player Player, Province From, Province To, Soldiers Soldiers) : ICommand
	{
		public bool Allowed(PlayersAndProvinces pap) => From.CanMove(pap, To, Player, Soldiers);
		public Province Perform(Province province)
		{
			return province == From ? province.Subtract(Soldiers) : province == To ? province.Add(new Manoeuvre(Player, Soldiers)) : province;
		}
		public bool HasEnoughCapacity(PlayersAndProvinces pap) => Soldiers.Capacity(pap, From, To) >= 0;
	}
}