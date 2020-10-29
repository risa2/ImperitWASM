using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayerPower : IEntity<PlayerPower, bool>
	{
		[Key] public int Id { get; set; }
		public int TurnIndex { get; set; }
		public int PlayerIndex { get; set; }
		public bool Alive { get; set; }
		public int Soldiers { get; set; }
		public int Lands { get; set; }
		public int Income { get; set; }
		public int Money { get; set; }
		public int Final { get; set; }
		public PlayerPower Convert(bool i) => new PlayerPower(Alive, Soldiers, Lands, Income, Money, Final);
		public static EntityPlayerPower From(PlayerPower pp) => new EntityPlayerPower { Alive = pp.Alive, Soldiers = pp.Soldiers, Lands = pp.Lands, Income = pp.Income, Money = pp.Money, Final = pp.Final };
	}
}
