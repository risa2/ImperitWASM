using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvinceAction : IEntity<IProvinceAction, (Settings, IReadOnlyList<Player>)>
	{
		[Key] public int Id { get; set; }
		public string Type { get; set; } = "";
		public EntityArmy Army { get; set; } = new EntityArmy();
		public IProvinceAction Convert((Settings, IReadOnlyList<Player>) arg)
		{
			var (settings, players) = arg;
			return Type switch
			{
				"Manoeuvre" => new Manoeuvre(Army.Convert((players, settings.SoldierTypes))),
				_ => throw new System.Exception("Unknown type of Action: " + Type)
			};
		}
		public static EntityProvinceAction From(IProvinceAction action) => action switch
		{
			Manoeuvre M => new EntityProvinceAction { Type = "Manoeuvre", Army = EntityArmy.From(M.Army) },
			_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
		};
	}
}