using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public record ProvinceData(string Name, ProvinceData.Kind Type, Shape Shape, ImmutableArray<int> Neighbors, ImmutableArray<ProvinceData.RegimentData> Soldiers, ImmutableArray<int> ExtraTypes = default, int? Earnings = null, bool? IsStart = null, bool? IsFinal = null, bool? HasPort = null)
	{
		public record RegimentData(int Type, int Count);
		public enum Kind { Land = 0, Sea = 1, Mountains = 2 }
		public Province Build(Settings set, Player player, Soldiers? soldiers = null, ImmutableList<IProvinceAction>? actions = null)
		{
			var defaultSoldiers = new Soldiers(Soldiers.Select(p => new Regiment(set.SoldierTypes[p.Type], p.Count)));
			return Type switch
			{
				Kind.Sea => new Sea(Name, Shape, player, soldiers ?? defaultSoldiers, defaultSoldiers, actions ?? ImmutableList<IProvinceAction>.Empty, set),
				Kind.Land => new Land(Name, Shape, player, soldiers ?? defaultSoldiers, defaultSoldiers, actions ?? ImmutableList<IProvinceAction>.Empty, set, Earnings ?? 0, IsStart ?? false, IsFinal ?? false, HasPort ?? false, ExtraTypes.Select(t => set.SoldierTypes[t]).ToImmutableArray()),
				_ => new Mountains(Name, Shape, set)
			};
		}
	}
}
