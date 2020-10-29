using System.Collections.Generic;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class JsonProvinceAction : IEntity<IProvinceAction, (Settings, IReadOnlyList<Player>)>
	{
		public string? Type { get; set; }
		public JsonArmy? Army { get; set; }
		public JsonSoldiers? Soldiers { get; set; }
		public IProvinceAction Convert(int i, (Settings, IReadOnlyList<Player>) arg)
		{
			var (settings, players) = arg;
			return Type switch
			{
				"Manoeuvre" => new Manoeuvre(Army!.Convert(i, (players, settings.SoldierTypes))),
				_ => throw new System.Exception("Unknown type of Action: " + Type)
			};
		}
		public static JsonProvinceAction From(IProvinceAction action)
		{
			return action switch
			{
				Manoeuvre M => new JsonProvinceAction { Type = "Manoeuvre", Army = JsonArmy.From(M.Army) },
				_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
			};
		}
	}
}