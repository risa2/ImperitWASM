using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ImperitWASM.Server.Load
{
	public class JsonProvince : IEntity<Province, (Settings, IReadOnlyList<Player>, IReadOnlyList<Shape>)>
	{
		public string Type { get; set; } = "";
		public string Name { get; set; } = "";
		public JsonArmy Army { get; set; } = new JsonArmy();
		public JsonArmy DefaultArmy { get; set; } = new JsonArmy();
		public int? Earnings { get; set; }
		public bool? IsStart { get; set; }
		public bool? IsFinal { get; set; }
		public IEnumerable<JsonProvinceAction>? Actions { get; set; }
		public Province Convert(int i, (Settings, IReadOnlyList<Player>, IReadOnlyList<Shape>) arg)
		{
			var (settings, players, shapes) = arg;
			return Type switch
			{
				"S" => new Sea(i, Name, shapes[i], Army.Convert(i, (players, settings.SoldierTypes)), DefaultArmy.Convert(i, (players, settings.SoldierTypes)), Actions?.Select((a, i) => a.Convert(i, (settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings),
				"L" => new Land(i, Name, shapes[i], Army.Convert(i, (players, settings.SoldierTypes)), DefaultArmy.Convert(i, (players, settings.SoldierTypes)), IsStart ?? false, Earnings ?? 0, Actions?.Select((a, i) => a.Convert(i, (settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings, IsFinal ?? false),
				"P" => new Port(i, Name, shapes[i], Army.Convert(i, (players, settings.SoldierTypes)), DefaultArmy.Convert(i, (players, settings.SoldierTypes)), IsStart ?? false, Earnings ?? 0, Actions?.Select((a, i) => a.Convert(i, (settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings, IsFinal ?? false),
				"M" => new Mountains(i, Name, shapes[i], Army.Convert(i, (players, settings.SoldierTypes)), DefaultArmy.Convert(i, (players, settings.SoldierTypes)), Actions?.Select((a, i) => a.Convert(i, (settings, players))).ToImmutableList() ?? ImmutableList<IProvinceAction>.Empty, settings),
				_ => throw new System.Exception("Unknown State.Province type: " + Type)
			};
		}
		public static JsonProvince From(Province prov) => prov switch
		{
			Port P => new JsonProvince { Type = "P", Name = P.Name, Army = JsonArmy.From(P.Army), DefaultArmy = JsonArmy.From(P.DefaultArmy), IsStart = P.IsStart, Earnings = P.Earnings, Actions = P.Actions.Select(JsonProvinceAction.From), IsFinal = P.IsFinal },
			Land L => new JsonProvince { Type = "L", Name = L.Name, Army = JsonArmy.From(L.Army), DefaultArmy = JsonArmy.From(L.DefaultArmy), IsStart = L.IsStart, Earnings = L.Earnings, Actions = L.Actions.Select(JsonProvinceAction.From), IsFinal = L.IsFinal },
			Sea S => new JsonProvince { Type = "S", Name = S.Name, Army = JsonArmy.From(S.Army), DefaultArmy = JsonArmy.From(S.DefaultArmy), Actions = S.Actions.Select(JsonProvinceAction.From) },
			Mountains M => new JsonProvince { Type = "M", Name = M.Name, Army = JsonArmy.From(M.Army), DefaultArmy = JsonArmy.From(M.DefaultArmy), Actions = M.Actions.Select(JsonProvinceAction.From) },
			_ => throw new System.Exception("Unknown State.Province type: " + prov.GetType())
		};
	}
}