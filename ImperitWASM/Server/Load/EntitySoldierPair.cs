using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldierPair : IEntity<(SoldierType, int), IReadOnlyList<SoldierType>>
	{
		[Key] public int Id { get; set; }
		public int Type { get; set; }
		public int Count { get; set; }
		public (SoldierType, int) Convert(IReadOnlyList<SoldierType> types) => (types[Type], Count);
		public static EntitySoldierPair From((SoldierType, int) p) => new EntitySoldierPair { Type = p.Item1.Id, Count = p.Item2 };
	}
}
