namespace ImperitWASM.Client.Data
{
	public class PurchaseInfo
	{
		public bool Possible { get; set; }
		public string Name { get; set; } = "";
		public int Price { get; set; }
		public int Money { get; set; }
		public PurchaseInfo(bool possible, string landName, int price, int money)
		{
			Possible = possible;
			Name = landName;
			Price = price;
			Money = money;
		}
		public PurchaseInfo() { }
	}
}
