namespace ImperitWASM.Shared.State
{
	public record Regiment(SoldierType Type, int Count)
	{
		public int AttackPower => Count * Type.AttackPower;
		public int DefensePower => Count * Type.DefensePower;
		public int Power => Count * Type.Power;
		public int Weight => Count * Type.Weight;
		public int Price => Count * Type.Price;

		public int CanMove(PlayersAndProvinces pap, Province from, Province to) => Type.CanMove(pap, from, to) * Count;
		public bool CanMoveAlone(PlayersAndProvinces pap, Province from, Province to) => Type.CanMoveAlone(pap, from, to);
		public int CanSustain(Province province) => Type.CanSustain(province) * Count;
	}
}
