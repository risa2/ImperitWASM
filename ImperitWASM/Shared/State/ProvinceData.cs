using System;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Motion;

namespace ImperitWASM.Shared.State
{
	public class ProvinceData
	{
		public enum Kind { Land, Sea, Mountains }
		public readonly string Name;
		public readonly Kind Type;
		public readonly Shape Shape;
		public readonly ImmutableArray<Tuple<int, int>> Soldiers;
		public readonly ImmutableArray<int> ExtraTypes;
		public readonly int? Earnings;
		public readonly bool? IsStart, IsFinal, HasPort;
		public ProvinceData(string name, Kind type, Shape shape, ImmutableArray<Tuple<int, int>> soldiers, ImmutableArray<int> extraTypes, int? earnings, bool? isStart, bool? isFinal, bool? hasPort)
		{
			Name = name;
			Type = type;
			Shape = shape;
			Soldiers = soldiers;
			ExtraTypes = extraTypes;
			Earnings = earnings;
			IsStart = isStart;
			IsFinal = isFinal;
			HasPort = hasPort;
		}
		public Province Build(Settings set, Player player, Soldiers? soldiers = null, ImmutableList<IProvinceAction>? actions = null)
		{
			var defaultSoldiers = new Soldiers(Soldiers.Select(p => (set.SoldierTypes[p.Item1], p.Item2)));
			return Type switch
			{
				Kind.Sea => new Sea(Name, Shape, player, soldiers ?? defaultSoldiers, defaultSoldiers, actions ?? ImmutableList<IProvinceAction>.Empty, set),
				Kind.Land => new Land(Name, Shape, player, soldiers ?? defaultSoldiers, defaultSoldiers, actions ?? ImmutableList<IProvinceAction>.Empty, set, Earnings ?? 0, IsStart ?? false, IsFinal ?? false, HasPort ?? false, ExtraTypes.Select(t => set.SoldierTypes[t]).ToImmutableArray()),
				_ => new Mountains(Name, Shape, set)
			};
		}
	}
}
