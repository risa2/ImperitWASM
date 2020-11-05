using System.Collections.Generic;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldierType : IEntity<SoldierType, IReadOnlyList<SoldierType>>
	{
		public int Id { get; set; }
		public int Type { get; set; }
		public SoldierType Convert(IReadOnlyList<SoldierType> arg) => arg[Type];
	}
}
