using System.ComponentModel.DataAnnotations;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayerPower : IEntity<PlayerPower, bool>
	{
		[Key] public int Id { get; set; }
		[Required] public int GameId { get; set; }
		[Required] public int TurnIndex { get; set; }
		[Required] public int PlayerIndex { get; set; }
		[Required] public bool Alive { get; set; }
		[Required] public int Soldiers { get; set; }
		[Required] public int Lands { get; set; }
		[Required] public int Income { get; set; }
		[Required] public int Money { get; set; }
		[Required] public int Final { get; set; }
		public PlayerPower Convert(bool i) => new PlayerPower(Alive, Soldiers, Lands, Income, Money, Final);
		public static EntityPlayerPower From(PlayerPower pp, int gameId, int turn, int player) => new EntityPlayerPower { Alive = pp.Alive, Soldiers = pp.Soldiers, Lands = pp.Lands, Income = pp.Income, Money = pp.Money, Final = pp.Final, GameId = gameId, PlayerIndex = player, TurnIndex = turn };
	}
}
