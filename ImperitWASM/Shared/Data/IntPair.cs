namespace ImperitWASM.Shared.Data
{
	public class IntPair
	{
		public IntPair(int a, int b) => (A, B) = (a, b);
		public IntPair() { }
		public int A { get; set; }
		public int B { get; set; }
		public void Deconstruct(out int a, out int b) => (a, b) = (A, B);
	}
}
