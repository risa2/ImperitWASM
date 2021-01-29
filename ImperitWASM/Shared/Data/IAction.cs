using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public interface IAction
	{
		(Player, Provinces, IAction?) Perform(Player active, Provinces provinces, Settings settings);
	}
}