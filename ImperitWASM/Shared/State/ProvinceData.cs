using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.ProvinceDataConverter))]
	public record ProvinceData(string Name, ProvinceData.Kind Type, Shape Shape, ImmutableArray<ProvinceData.RegimentData> Soldiers, ImmutableArray<int> ExtraTypes, int? Earnings, bool? IsStart, bool? IsFinal, bool? HasPort)
	{
		public record RegimentData(int Type, int Count);
		public enum Kind { Land, Sea, Mountains }
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
