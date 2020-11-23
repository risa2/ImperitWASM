using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvince : IEntity
	{
		[Key] public int Id { get; set; }
		public Game? Game { get; set; }
		public int GameId { get; set; }
		public int Index { get; set; }
		public EntityPlayer? EntityPlayer { get; set; }
		public int EntityPlayerId { get; set; }
		public EntitySoldier? EntitySoldier { get; set; }
		public int EntitySoldierId { get; set; }
		public ICollection<EntityProvinceAction>? EntityProvinceActions { get; set; }
		public Province Convert(Settings set)
		{
			return set.ProvinceData[Index].Build(set, EntityPlayer!.Convert(set), EntitySoldier!.Convert(set.SoldierTypes), EntityProvinceActions?.Select(a => a.Convert(set))?.ToImmutableList());
		}
		public static EntityProvince From(Province p, IReadOnlyDictionary<Player, EntityPlayer> map, IReadOnlyDictionary<SoldierType, int> smap, int index)
		{
			return new EntityProvince { Index = index, EntityPlayer = map[p.Player], EntitySoldier = EntitySoldier.From(p.Soldiers, smap), EntityProvinceActions = p.Actions.Select(a => EntityProvinceAction.From(a, map, smap)).ToList() };
		}
	}
}