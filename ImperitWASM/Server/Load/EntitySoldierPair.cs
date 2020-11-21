using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldierPair : IEntity
	{
		[Key] public int Id { get; set; }
		public EntitySoldier? EntitySoldier { get; set; }
		public int EntitySoldierId { get; set; }
		public int Type { get; set; }
		public int Count { get; set; }
		public (SoldierType, int) Convert(IReadOnlyList<SoldierType> types) => (types[Type], Count);
		public static EntitySoldierPair From((SoldierType, int) p, IReadOnlyDictionary<SoldierType, int> map) => new EntitySoldierPair { Type = map[p.Item1], Count = p.Item2 };
	}
}
