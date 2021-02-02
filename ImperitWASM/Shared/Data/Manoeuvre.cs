using System.Linq;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Manoeuvre(int Province, Soldiers Soldiers) : IAction
	{
		public (Player, Provinces, IAction?) Perform(Player active, Provinces provinces, Settings settings)
		{
			return (active, provinces.With(provinces.Select(altered => altered.Order == Province ? altered.VisitedBy(active.Id, Soldiers) : altered)), null);
		}
	}
}