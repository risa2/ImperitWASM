using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class Ship : SoldierType
	{
		public int Capacity { get; }
		public Ship(string name, string text, string symbol, int attackPower, int defensePower, int weight, int price, int capacity)
			: base(name, text, symbol, attackPower, defensePower, weight, price) => Capacity = capacity;
		protected static bool IsPassable(Province p) => (p is Land l && l.HasPort) || p is Sea;
		public override int CanMove(PlayersAndProvinces pap, Province from, Province dest)
		{
			return pap.Passable(from, dest, 1, (a, b) => IsPassable(a) && IsPassable(b) ? 1 : 2) ? Capacity + Weight : 0;
		}
		public override bool IsRecruitable(Province p) => p is Land l && l.HasPort;
		public override int CanSustain(Province province) => province is Sea ? Capacity + Weight : (province is Land l && l.HasPort) ? Weight : 0;
	}
}
