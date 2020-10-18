namespace ImperitWASM.Shared.Data
{
	public class PlayerId
	{
		public PlayerId(int id, string name)
		{
			I = id;
			N = name;
		}
		public PlayerId() { }
		public int I { get; set; }
		public string N { get; set; } = "";
	}
}
