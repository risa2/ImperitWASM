namespace ImperitWASM.Shared.Data
{
	public class User
	{
		public int Id { get; set; }
		public string LoginId { get; set; } = "";
		public User() { }
		public User(int id, string loginId)
		{
			Id = id;
			LoginId = loginId;
		}
	}
}
