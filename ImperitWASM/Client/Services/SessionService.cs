using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace ImperitWASM.Client.Services
{
	public class SessionService
	{
		readonly ISessionStorageService iss;
		readonly string number_name, login_name;
		public SessionService(ISessionStorageService iss, string number_name, string login_name)
		{
			this.iss = iss;
			this.number_name = number_name;
			this.login_name = login_name;
		}
		public async Task<Shared.Data.User?> ReadUser()
		{
			var (number, login) = (await iss.GetItemAsync<int?>(number_name), await iss.GetItemAsync<string?>(login_name));
			return number is int n && login is string s ? new Shared.Data.User(n, s) : null;
		}
		public async Task WriteUser(Shared.Data.User? user)
		{
			await iss.SetItemAsync(number_name, user?.U);
			await iss.SetItemAsync(login_name, user?.I);
		}
		public Task Clear() => iss.ClearAsync();
	}
}
