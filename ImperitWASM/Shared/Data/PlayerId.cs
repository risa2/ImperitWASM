namespace ImperitWASM.Shared.Data
{
	public class PlayerId
	{
		public PlayerId(int id, string name)
		{
			Id = id;
			Name = name;
		}
		public PlayerId() { }
		public int Id { get; set; }
		public string Name { get; set; } = "";
	}
}
