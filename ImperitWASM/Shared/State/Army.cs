namespace ImperitWASM.Shared.State
{
	public class Army
	{
		public Soldiers Soldiers { get; }
		public Player Player;
		public Color Color => Player.Color;
		public bool IsControlledBySavage => Player is Savage;
		public Army(Soldiers soldiers, Player plr)
		{
			Soldiers = soldiers;
			Player = plr;
		}
		public Army Join(Soldiers another) => new Army(Soldiers.Add(another), Player);
		public Army Subtract(Soldiers another) => new Army(Soldiers.Subtract(another), Player);
		public bool IsAllyOf(Player plr) => Player == plr;
		public bool IsAllyOf(Army another) => another.IsAllyOf(Player);
		public int AttackPower => Soldiers.AttackPower;
		public int DefensePower => Soldiers.DefensePower;
		public int Price => Soldiers.Price;
		public bool AnySoldiers => Soldiers.Any;
		public Army AttackedBy(Army another)
		{
			var soldiers = Soldiers.AttackedBy(another.Soldiers);
			return DefensePower >= another.AttackPower ? new Army(soldiers, Player) : new Army(soldiers, another.Player);
		}
		public bool CanMove(IProvinces provinces, int from, int to) => Soldiers.CanMove(provinces, from, to);
		public override string ToString() => Soldiers.ToString();
	}
}