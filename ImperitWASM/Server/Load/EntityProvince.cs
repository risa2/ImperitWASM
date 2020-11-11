using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvince : IEntity
	{
		[Key] public int Id { get; set; }
		public EntityGame EntityGame { get; set; } = new EntityGame();
		public int EntityGameId { get; set; }
		public int Index { get; set; }
		public string Type { get; set; } = "";
		public string Name { get; set; } = "";
		public int Player { get; set; }
		public EntitySoldier EntitySoldier { get; set; } = new EntitySoldier();
		public int EntitySoldierId { get; set; }
		public int? Earnings { get; set; }
		public bool? IsStart { get; set; }
		public bool? IsFinal { get; set; }
		public bool? HasPort { get; set; }
		public IEnumerable<EntityProvinceAction>? EntityProvinceActions { get; set; }
		public IEnumerable<EntitySoldierType>? EntitySoldierTypes { get; set; }
		public Province Convert(Settings set, IReadOnlyList<Player> players) => Type switch
		{
			"S" => new Sea(Name, set.Shapes[Index], players[Player], EntitySoldier.Convert(set.SoldierTypes), new Soldiers(set.DefaultSoldiers[Index].Select(p => (set.SoldierTypes[p.Item1], p.Item2))), EntityProvinceActions?.Select(a => a.Convert(set, players)).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, set),
			"L" => new Land(Name, set.Shapes[Index], players[Player], EntitySoldier.Convert(set.SoldierTypes), new Soldiers(set.DefaultSoldiers[Index].Select(p => (set.SoldierTypes[p.Item1], p.Item2))), EntityProvinceActions?.Select(a => a.Convert(set, players)).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, set, Earnings ?? 0, IsStart ?? false, IsFinal ?? false, HasPort ?? false),
			"O" => new Outland(Name, set.Shapes[Index], players[Player], EntitySoldier.Convert(set.SoldierTypes), new Soldiers(set.DefaultSoldiers[Index].Select(p => (set.SoldierTypes[p.Item1], p.Item2))), EntityProvinceActions?.Select(a => a.Convert(set, players)).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, set, Earnings ?? 0, IsFinal ?? false, EntitySoldierTypes.Select(t => t.Convert(set.SoldierTypes)).ToImmutableArray()),
			"M" => new Mountains(Name, set.Shapes[Index], set),
			_ => throw new System.Exception("Unknown State.Province type: " + Type)
		};
		public EntityProvince Assign(Province p, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap)
		{
			_ = p switch
			{
				Outland O => (Type = "P", Name = O.Name, Player = map[O.Player], EntitySoldier = EntitySoldier.From(O.Soldiers, smap), IsStart = O.IsStart, Earnings = O.Earnings, EntityProvinceActions = O.Actions.Select(a => EntityProvinceAction.From(a, map, smap)), IsFinal = O.IsFinal),
				Land L => (Type = "L", Name = L.Name, Player = map[L.Player], EntitySoldier = EntitySoldier.From(L.Soldiers, smap), IsStart = L.IsStart, Earnings = L.Earnings, EntityProvinceActions = L.Actions.Select(a => EntityProvinceAction.From(a, map, smap)), IsFinal = L.IsFinal),
				Sea S => (Type = "S", Name = S.Name, Player = map[S.Player], EntitySoldier = EntitySoldier.From(S.Soldiers, smap), EntityProvinceActions = S.Actions.Select(a => EntityProvinceAction.From(a, map, smap))),
				_ => (object)(Type = "M")
			};
			return this;
		}
		public static EntityProvince From(Province p, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap, int gameId, int index)
		{
			return new EntityProvince { EntityGameId = gameId, Index = index }.Assign(p, map, smap);
		}
	}
}