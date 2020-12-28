namespace ImperitWASM.Shared.Data
{
	public record Manoeuvre(Player Player, Soldiers Soldiers) : IProvinceAction
	{
		public (Province, IProvinceAction?) Perform(Province province, PlayersAndProvinces pap)
		{
			return (province.IsAllyOf(Player) ? province.Reinforce(Soldiers) : province.AttackedBy(Player, Soldiers), null);
		}
	}
}