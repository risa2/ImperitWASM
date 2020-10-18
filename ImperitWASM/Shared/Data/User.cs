namespace ImperitWASM.Shared.Data
{
	public class User
	{
		public int U { get; set; }
		public string I { get; set; } = "";
		public User() { }
		public User(int id, string loginId)
		{
			U = id;
			I = loginId;
		}
	}
}
