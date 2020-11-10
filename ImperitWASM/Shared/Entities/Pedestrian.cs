using ImperitWASM.Shared.Func;

namespace ImperitWASM.Shared.Entities
{
	public class Pedestrian : SoldierType
	{
		public Pedestrian(string name, string text, string symbol, int attackPower, int defensePower, int weight, int price)
			: base(name, text, symbol, attackPower, defensePower, weight, price) { }
		public override int CanMove(PlayersAndProvinces pap, Province from, Province to)
		{
			return from is Land && to is Land && pap.Passable(from, to, 1, (a, b) => a is Land && b is Land ? 1 : 2) ? Weight : 0;
		}
		public override bool IsRecruitable(Province province) => province is Land;
		public override int CanSustain(Province province) => province is Land ? Weight : 0;
	}
}
