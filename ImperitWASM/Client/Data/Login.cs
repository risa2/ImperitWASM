namespace ImperitWASM.Client.Data
{
	public class Login
	{
		public string N { get; set; } = "";
		public string P { get; set; } = "";
		public Login(string name, string password) => (N, P) = (name, password);
		public Login() { }
	}
}
