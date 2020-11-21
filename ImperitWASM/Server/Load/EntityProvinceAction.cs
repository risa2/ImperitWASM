using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvinceAction : IEntity
	{
		[Key] public int Id { get; set; }
		public EntityProvince? EntityProvince { get; set; }
		public int EntityProvinceId { get; set; }
		public string Type { get; set; } = "";
		public int Player { get; set; }
		public EntitySoldier? EntitySoldier { get; set; }
		public int EntitySoldierId { get; set; }
		public IProvinceAction Convert(Settings settings, IReadOnlyList<Player> players) => Type switch
		{
			"Manoeuvre" => new Manoeuvre(players[Player], EntitySoldier!.Convert(settings.SoldierTypes)),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static EntityProvinceAction From(IProvinceAction action, IReadOnlyDictionary<Player, int> map, IReadOnlyDictionary<SoldierType, int> smap) => action switch
		{
			Manoeuvre M => new EntityProvinceAction { Type = "Manoeuvre", Player = map[M.Player], EntitySoldier = EntitySoldier.From(M.Soldiers, smap) },
			_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
		};
	}
}