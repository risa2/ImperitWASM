using ImperitWASM.Shared.Config;

namespace ImperitWASM.Shared.Data
{
	public record Regiment(SoldierType Type, int Count)
	{
		public int AttackPower => Count * Type.AttackPower;
		public int DefensePower => Count * Type.DefensePower;
		public int Power => Count * Type.Power;
		public int Weight => Count * Type.Weight;
		public int Price => Count * Type.Price;

		public int CanMove(Provinces provinces, Province from, Province to) => Type.CanMove(provinces, from, to) * Count;
		public bool CanMoveAlone(Provinces provinces, Province from, Province to) => Type.CanMoveAlone(provinces, from, to);
		public int CanSustain(Region province) => Type.CanSustain(province) * Count;
	}
}
