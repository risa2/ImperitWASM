using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Load
{
	public class EntityProvinceAction : IEntity
	{
		public enum Kind { Manoeuvre, Existence }
		[Key] public int Id { get; set; }
		public EntityProvince? EntityProvince { get; set; }
		public int EntityProvinceId { get; set; }
		public Kind Type { get; set; }
		public EntityPlayer? EntityPlayer { get; set; }
		public int EntityPlayerId { get; set; }
		public ICollection<EntitySoldier>? EntitySoldiers { get; set; }
		public IProvinceAction Convert(Settings settings) => Type switch
		{
			Kind.Manoeuvre => new Manoeuvre(EntityPlayer!.Convert(settings), GetSoldiers(settings.SoldierTypes)),
			_ => throw new System.Exception("Unknown type of Action: " + Type)
		};
		public static EntityProvinceAction From(IProvinceAction action, IReadOnlyDictionary<Player, EntityPlayer> map, IReadOnlyDictionary<SoldierType, int> smap) => action switch
		{
			Manoeuvre M => new EntityProvinceAction { Type = Kind.Manoeuvre, EntityPlayer = map[M.Player], EntitySoldiers = FromSoldiers(M.Soldiers, smap) },
			_ => throw new System.Exception("Unknown type of Action: " + action.GetType())
		};
		public Soldiers GetSoldiers(IReadOnlyList<SoldierType> types) => new Soldiers(EntitySoldiers!.Select(item => item.Convert(types)));
		static List<EntitySoldier> FromSoldiers(Soldiers soldiers, IReadOnlyDictionary<SoldierType, int> map) => soldiers.Select(s => EntitySoldier.From(s, map)).ToList();
		public static EntityProvinceAction From(Soldiers soldiers, EntityPlayer player, IReadOnlyDictionary<SoldierType, int> smap) => new EntityProvinceAction { Type = Kind.Existence, EntityPlayer = player, EntitySoldiers = FromSoldiers(soldiers, smap) };
	}
}