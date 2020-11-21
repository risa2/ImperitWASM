using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace ImperitWASM.Client.Services
{
	public class SessionService
	{
		private readonly ISessionStorageService iss;
		public SessionService(ISessionStorageService iss) => this.iss = iss;
		public Task<Data.Session?> GetAsync() => iss.GetItemAsync<Data.Session?>("session");
		public Task SetAsync(Data.Session? session) => iss.SetItemAsync("session", session);
		public Task RemoveAsync() => iss.RemoveItemAsync("session");
	}
}
