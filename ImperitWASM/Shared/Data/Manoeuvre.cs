using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Manoeuvre(Province Province, Soldiers Soldiers) : IAction
	{
		public (Player, Provinces, IAction?) Perform(Player active, Provinces provinces, Settings settings)
		{
			return (active, provinces.With(provinces.Select(altered => altered == Province ? altered.VisitedBy(active, Soldiers) : altered)), null);
		}
	}
}