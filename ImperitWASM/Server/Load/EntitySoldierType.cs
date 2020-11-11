using System.Collections.Generic;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldierType : IEntity
	{
		public int Id { get; set; }
		public int Type { get; set; }
		public EntityProvince EntityProvince { get; set; } = new EntityProvince();
		public int EntityProvinceId { get; set; }
		public SoldierType Convert(IReadOnlyList<SoldierType> arg) => arg[Type];
	}
}
