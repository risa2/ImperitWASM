using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.Entities;

namespace ImperitWASM.Shared.Cfg
{
	[JsonConverter(typeof(Cvt.ProvinceFactoryConverter))]
	public class ProvinceFactory
	{
		public enum Kind { Land, Sea, Mountains }
		public readonly Kind Type;
		public readonly string Name = "";
		public readonly Shape Shape;
		public readonly Soldiers Soldiers;
		public readonly int Earnings;
		public readonly bool IsStart, IsFinal, HasPort;
		public readonly ImmutableArray<int> ExtraTypes;
		public ProvinceFactory(Kind type, string name, Shape shape, Soldiers soldiers, int earnings, bool isStart, bool isFinal, bool hasPort, ImmutableArray<int> extraTypes)
		{
			Type = type;
			Name = name;
			Shape = shape;
			Soldiers = soldiers;
			Earnings = earnings;
			IsStart = isStart;
			IsFinal = isFinal;
			HasPort = hasPort;
			ExtraTypes = extraTypes;
		}
		public Province Create(Settings set, Player player) => Type switch
		{
			Kind.Land => new Land(Name, Shape, player, Soldiers, Soldiers, ImmutableList<ProvinceAction>.Empty, Earnings, IsStart, IsFinal, HasPort, ExtraTypes.Select(i => set.SoldierTypes[i]).ToImmutableArray()),
			Kind.Sea => new Sea(Name, Shape, player, Soldiers, Soldiers, ImmutableList<ProvinceAction>.Empty),
			Kind.Mountains => new Mountains(Name, Shape),
			_ => throw new System.Exception("Invalid ProvinceFactory.Kind")
		};
	}
}
