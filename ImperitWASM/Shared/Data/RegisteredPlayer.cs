namespace ImperitWASM.Shared.Data
{
	public class RegisteredPlayer
	{
		public string Name { get; set; } = "";
		public string Password { get; set; } = "";
		public int Start { get; set; }
		public RegisteredPlayer() { }
		public RegisteredPlayer(string name, string password, int start)
		{
			Name = name;
			Password = password;
			Start = start;
		}
	}
}
