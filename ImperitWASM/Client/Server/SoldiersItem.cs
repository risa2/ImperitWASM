using ImperitWASM.Shared.Cfg;

namespace ImperitWASM.Client.Server
{
	public class SoldiersItem
	{
		public SoldiersItem(Description description, int count)
		{
			D = description;
			P = count;
		}
		public SoldiersItem() { }
		public Description D { get; set; } = new Description();
		public int P { get; set; }
		public void Deconstruct(out Description d, out int c) => (d, c) = (D, P);
	}
}
