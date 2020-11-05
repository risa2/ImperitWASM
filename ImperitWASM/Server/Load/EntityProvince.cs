using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvince : IEntity<Province, (Settings, IReadOnlyList<Shape>, IReadOnlyList<Player>)>
	{
		[Key] public int Id { get; set; }
		[Required] public int GameId { get; set; }
		[Required] public int Index { get; set; }
		[Required] public string Type { get; set; } = "";
		[Required] public string Name { get; set; } = "";
		[Required] public int Player { get; set; }
		[Required] public EntitySoldiers Soldiers { get; set; } = new EntitySoldiers();
		[Required] public EntitySoldiers DefaultSoldiers { get; set; } = new EntitySoldiers();
		public int? Earnings { get; set; }
		public bool? IsStart { get; set; }
		public bool? IsFinal { get; set; }
		public bool? HasPort { get; set; }
		public IEnumerable<EntityProvinceAction>? Actions { get; set; }
		public IEnumerable<EntitySoldierType>? ExtraTypes { get; set; }
		public Province Convert((Settings, IReadOnlyList<Shape>, IReadOnlyList<Player>) arg)
		{
			var (settings, shapes, players) = arg;
			return Type switch
			{
				"S" => new Sea(Name, shapes[Index], players[Player], Soldiers.Convert(settings.SoldierTypes), DefaultSoldiers.Convert(settings.SoldierTypes), Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings),
				"L" => new Land(Name, shapes[Index], players[Player], Soldiers.Convert(settings.SoldierTypes), DefaultSoldiers.Convert(settings.SoldierTypes), Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings, Earnings ?? 0, IsStart ?? false, IsFinal ?? false, HasPort ?? false),
				"O" => new Outland(Name, shapes[Index], players[Player], Soldiers.Convert(settings.SoldierTypes), DefaultSoldiers.Convert(settings.SoldierTypes), Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings, Earnings ?? 0, IsFinal ?? false, ExtraTypes.Select(t => t.Convert(settings.SoldierTypes)).ToImmutableArray()),
				"M" => new Mountains(shapes[Index], settings),
				_ => throw new System.Exception("Unknown State.Province type: " + Type)
			};
		}
		public EntityProvince Assign(Province p, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap)
		{
			_ = p switch
			{
				Outland O => (Type = "P", Name = O.Name, Player = map[O.Player], Soldiers = EntitySoldiers.From(O.Soldiers, smap), DefaultSoldiers = EntitySoldiers.From(O.DefaultSoldiers, smap), IsStart = O.IsStart, Earnings = O.Earnings, Actions = O.Actions.Select(a => EntityProvinceAction.From(a, map, smap)), IsFinal = O.IsFinal),
				Land L => (Type = "L", Name = L.Name, Player = map[L.Player], Soldiers = EntitySoldiers.From(L.Soldiers, smap), DefaultSoldiers = EntitySoldiers.From(L.DefaultSoldiers, smap), IsStart = L.IsStart, Earnings = L.Earnings, Actions = L.Actions.Select(a => EntityProvinceAction.From(a, map, smap)), IsFinal = L.IsFinal),
				Sea S => (Type = "S", Name = S.Name, Player = map[S.Player], Soldiers = EntitySoldiers.From(S.Soldiers, smap), DefaultSoldiers = EntitySoldiers.From(S.DefaultSoldiers, smap), Actions = S.Actions.Select(a => EntityProvinceAction.From(a, map, smap))),
				_ => (object)(Type = "M")
			};
			return this;
		}
		public static EntityProvince From(Province p, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap, int gameId, int index)
		{
			return new EntityProvince { GameId = gameId, Index = index }.Assign(p, map, smap);
		}
	}
}