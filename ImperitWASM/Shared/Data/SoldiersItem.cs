using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Data
{
	public class SoldiersItem
	{
		public SoldiersItem(Description description, int count)
		{
			Description = description;
			Price = count;
		}
		public SoldiersItem() { }
		public Description Description { get; set; } = new Description();
		public int Price { get; set; }
		public void Deconstruct(out Description d, out int c) => (d, c) = (Description, Price);
	}
}
