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
		public ICollection<EntityProvinceAction>? EntityProvinceActions { get; set; }
		public Province Convert(Settings set)
		{
			var basic = EntityProvinceActions!.Single(a => a.Type == EntityProvinceAction.Kind.Existence);
			return set.Provinces[Index].Build(set, basic.EntityPlayer!.Convert(set), basic.GetSoldiers(set.SoldierTypes), EntityProvinceActions?.OrderBy(a => a.Type)?.Where(a => a.Type != EntityProvinceAction.Kind.Existence)?.Select(a => a.Convert(set))?.ToImmutableList());
		}
		public static EntityProvince From(Province p, IReadOnlyDictionary<Player, EntityPlayer> map, IReadOnlyDictionary<SoldierType, int> smap, int index)
		{
			return new EntityProvince { Index = index, EntityProvinceActions = p.Actions.Select(a => EntityProvinceAction.From(a, map, smap)).Append(EntityProvinceAction.From(p.Soldiers, map[p.Player], smap)).ToList() };
		}
	}
}