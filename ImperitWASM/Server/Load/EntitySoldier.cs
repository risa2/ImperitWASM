using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntitySoldier : IEntity
	{
		[Key] public int Id { get; set; }
		public ICollection<EntitySoldierPair>? EntitySoldierPairs { get; set; }
		public Soldiers Convert(IReadOnlyList<SoldierType> types)
		{
			return new Soldiers(EntitySoldierPairs.Select(item => item.Convert(types)));
		}
		public static EntitySoldier From(Soldiers soldiers, IReadOnlyDictionary<SoldierType, int> map)
		{
			return new EntitySoldier { EntitySoldierPairs = soldiers.Select(s => EntitySoldierPair.From(s, map)).ToArray() };
		}
	}
}
