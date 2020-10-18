namespace ImperitWASM.Shared.Data
{
	public class PlayerFullInfo
	{
		public PlayerFullInfo() { }
		public PlayerFullInfo(int id, bool real, string name, string color, bool alive, int money, int income, int debt)
		{
			Id = id;
			Display = real;
			Name = name;
			Color = color;
			Alive = alive;
			Money = money;
			Income = income;
			Debt = debt;
		}
		public int Id { get; set; }
		public bool Display { get; set; }
		public string Name { get; set; } = "";
		public string Color { get; set; } = "";
		public bool Alive { get; set; }
		public int Money { get; set; }
		public int Income { get; set; }
		public int Debt { get; set; }
	}
}
