using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvinceAction : IEntity<IProvinceAction, (Settings settings, IReadOnlyList<Player> players)>
	{
		[Key] public int Id { get; set; }
		[Required] public string Type { get; set; } = "";
		[Required] public int Player { get; set; }
		[Required] public EntitySoldiers Soldiers { get; set; } = new EntitySoldiers();
		public IProvinceAction Convert((Settings settings, IReadOnlyList<Player> players) x) => Type switch
		{
			"Manoeuvre" => new Manoeuvre(x.players[Player], Soldiers.Convert(x.settings.SoldierTypes)),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static EntityProvinceAction From(IProvinceAction action, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap) => action switch
		{
			Manoeuvre M => new EntityProvinceAction { Type = "Manoeuvre", Player = map[M.Player], Soldiers = EntitySoldiers.From(M.Soldiers, smap) },
			_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
		};
	}
}