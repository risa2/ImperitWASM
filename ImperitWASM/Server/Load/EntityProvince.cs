using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvince : IEntity<Province, (Settings, IReadOnlyList<Player>, IReadOnlyList<Shape>)>
	{
		[Key] public int Id { get; set; }
		public int Index { get; set; }
		public string Type { get; set; } = "";
		public string Name { get; set; } = "";
		public EntityArmy Army { get; set; } = new EntityArmy();
		public EntityArmy DefaultArmy { get; set; } = new EntityArmy();
		public int? Earnings { get; set; }
		public bool? IsStart { get; set; }
		public bool? IsFinal { get; set; }
		public IEnumerable<EntityProvinceAction>? Actions { get; set; }
		public Province Convert((Settings, IReadOnlyList<Player>, IReadOnlyList<Shape>) arg)
		{
			var (settings, players, shapes) = arg;
			return Type switch
			{
				"S" => new Sea(Index, Name, shapes[Index], Army.Convert((players, settings.SoldierTypes)), DefaultArmy.Convert((players, settings.SoldierTypes)), Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings),
				"L" => new Land(Index, Name, shapes[Index], Army.Convert((players, settings.SoldierTypes)), DefaultArmy.Convert((players, settings.SoldierTypes)), IsStart ?? false, Earnings ?? 0, Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings, IsFinal ?? false),
				"P" => new Port(Index, Name, shapes[Index], Army.Convert((players, settings.SoldierTypes)), DefaultArmy.Convert((players, settings.SoldierTypes)), IsStart ?? false, Earnings ?? 0, Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings, IsFinal ?? false),
				"M" => new Mountains(Index, Name, shapes[Index], Army.Convert((players, settings.SoldierTypes)), DefaultArmy.Convert((players, settings.SoldierTypes)), Actions?.Select(a => a.Convert((settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings),
				_ => throw new System.Exception("Unknown State.Province type: " + Type)
			};
		}
		public static EntityProvince From(Province prov) => prov switch
		{
			Port P => new EntityProvince { Type = "P", Index = P.Id, Name = P.Name, Army = EntityArmy.From(P.Army), DefaultArmy = EntityArmy.From(P.DefaultArmy), IsStart = P.IsStart, Earnings = P.Earnings, Actions = P.Actions.Select(EntityProvinceAction.From), IsFinal = P.IsFinal },
			Land L => new EntityProvince { Type = "L", Index = L.Id, Name = L.Name, Army = EntityArmy.From(L.Army), DefaultArmy = EntityArmy.From(L.DefaultArmy), IsStart = L.IsStart, Earnings = L.Earnings, Actions = L.Actions.Select(EntityProvinceAction.From), IsFinal = L.IsFinal },
			Sea S => new EntityProvince { Type = "S", Index = S.Id, Name = S.Name, Army = EntityArmy.From(S.Army), DefaultArmy = EntityArmy.From(S.DefaultArmy), Actions = S.Actions.Select(EntityProvinceAction.From) },
			Mountains M => new EntityProvince { Type = "M", Index = M.Id, Name = M.Name, Army = EntityArmy.From(M.Army), DefaultArmy = EntityArmy.From(M.DefaultArmy), Actions = M.Actions.Select(EntityProvinceAction.From) },
			_ => throw new System.Exception("Unknown State.Province type: " + prov.GetType())
		};
	}
}