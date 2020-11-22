using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityProvinceAction : IEntity
	{
		public enum Kind { Manoeuvre }
		[Key] public int Id { get; set; }
		public EntityProvince? EntityProvince { get; set; }
		public int EntityProvinceId { get; set; }
		public Kind Type { get; set; }
		public EntityPlayer? EntityPlayer { get; set; }
		public int EntityPlayerId { get; set; }
		public EntitySoldier? EntitySoldier { get; set; }
		public int EntitySoldierId { get; set; }
		public IProvinceAction Convert(Settings settings) => Type switch
		{
			Kind.Manoeuvre => new Manoeuvre(EntityPlayer!.Convert(settings), EntitySoldier!.Convert(settings.SoldierTypes)),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static EntityProvinceAction From(IProvinceAction action, IReadOnlyDictionary<Player, EntityPlayer> map, IReadOnlyDictionary<SoldierType, int> smap) => action switch
		{
			Manoeuvre M => new EntityProvinceAction { Type = Kind.Manoeuvre, EntityPlayer = map[M.Player], EntitySoldier = EntitySoldier.From(M.Soldiers, smap) },
			_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
		};
	}
}