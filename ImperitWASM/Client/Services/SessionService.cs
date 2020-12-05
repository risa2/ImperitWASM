using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace ImperitWASM.Client.Services
{
	public class SessionService
	{
		readonly ISessionStorageService iss;
		public SessionService(ISessionStorageService iss) => this.iss = iss;
		public async Task<Data.Session> GetAsync() => await iss.GetItemAsync<Data.Session?>("session") ?? new Data.Session();
		public Task SetAsync(Data.Session session) => iss.SetItemAsync("session", session);
		public Task RemoveAsync() => iss.RemoveItemAsync("session");
	}
}
