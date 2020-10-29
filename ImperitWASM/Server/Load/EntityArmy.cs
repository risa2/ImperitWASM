using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityArmy : IEntity<Army, (IReadOnlyList<Player>, IReadOnlyList<SoldierType>)>
	{
		[Key] public int Id { get; set; }
		public EntitySoldiers Soldiers { get; set; } = new EntitySoldiers();
		public int Player { get; set; }
		public Army Convert((IReadOnlyList<Player>, IReadOnlyList<SoldierType>) arg)
		{
			var (players, types) = arg;
			return new Army(Soldiers.Convert(types), players[Player]);
		}
		public static EntityArmy From(Army army)
		{
			return new EntityArmy { Soldiers = EntitySoldiers.From(army.Soldiers), Player = army.Player.Id };
		}
	}
}