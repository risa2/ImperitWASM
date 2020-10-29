using System;
using System.Linq;
using System.Threading.Tasks;
using ImperitWASM.Server.Load;

namespace ImperitWASM.Server.Services
{
	public interface ILoginService
	{
		string Get(int user);
		Task Remove(int user);
		Task Reset(int len);
	}
	public class LoginService : ILoginService
	{
		readonly Random rand = new Random();
		readonly IFile file;
		string[] logins;
		public LoginService(IServiceIO io)
		{
			file = io.Sessions;
			logins = file.ReadJsons<string, string?>((x, i) => x ?? string.Empty).ToArray();
		}
		public string Get(int user) => user < 0 || user >= logins.Length ? string.Empty : logins[user];
		public Task Remove(int user)
		{
			if (user >= 0 && user < logins.Length)
			{
				logins[user] = rand.NextId(32);
			}
			return file.WriteJsons(logins, x => x);
		}
		public Task Reset(int len)
		{
			logins = new string[len];
			for (int i = 0; i < len; ++i)
			{
				logins[i] = rand.NextId(32);
			}
			return file.WriteJsons(logins, x => x);
		}
	}
}
