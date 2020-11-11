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
		public Game Game { get; set; } = new Game();
		public int GameId { get; set; }
		public int Index { get; set; }
		public int Player { get; set; }
		public EntitySoldier EntitySoldier { get; set; } = new EntitySoldier();
		public int EntitySoldierId { get; set; }
		public IEnumerable<EntityProvinceAction>? EntityProvinceActions { get; set; }
		public Province Convert(Settings set, IReadOnlyList<Player> players)
		{
			return set.ProvinceData[Index].Build(set, players[Player], EntitySoldier.Convert(set.SoldierTypes), EntityProvinceActions?.Select(a => a.Convert(set, players))?.ToImmutableList());
		}
		public EntityProvince Assign(Province p, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap)
		{
			Player = map[p.Player];
			EntitySoldier = EntitySoldier.From(p.Soldiers, smap);
			EntityProvinceActions = p.Actions.Select(a => EntityProvinceAction.From(a, map, smap));
			return this;
		}
		public static EntityProvince From(Province p, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap, int gameId, int index)
		{
			return new EntityProvince { GameId = gameId, Index = index }.Assign(p, map, smap);
		}
	}
}