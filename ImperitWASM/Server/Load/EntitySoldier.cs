using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldier : IEntity
	{
		[Key] public int Id { get; set; }
		public EntityProvinceAction? EntityProvinceAction { get; set; }
		public int EntityProvinceActionId { get; set; }
		public int Type { get; set; }
		public int Count { get; set; }
		public Regiment Convert(IReadOnlyList<SoldierType> types) => new Regiment(types[Type], Count);
		public static EntitySoldier From(Regiment p, IReadOnlyDictionary<SoldierType, int> map) => new EntitySoldier { Type = map[p.Type], Count = p.Count };
	}
}
