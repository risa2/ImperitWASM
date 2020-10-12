namespace ImperitWASM.Shared.Data
{
	public class Login
	{
		public Login(int id, string password)
		{
			Id = id;
			Password = password;
		}
		public Login() { }
		public int Id { get; set; }
		public string Password { get; set; } = "";
	}
}
