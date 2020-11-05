using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace ImperitWASM.Client.Services
{
	public class SessionService
	{
		readonly ISessionStorageService iss;
		public SessionService(ISessionStorageService iss) => this.iss = iss;
		public Task<Server.Session?> GetAsync() => iss.GetItemAsync<Server.Session?>("session");
		public Task SetAsync(Server.Session? session) => iss.SetItemAsync("session", session);
		public Task RemoveAsync() => iss.RemoveItemAsync("session");
	}
}
