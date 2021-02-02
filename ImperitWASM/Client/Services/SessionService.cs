using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace ImperitWASM.Client.Services
{
	public class SessionService
	{
		readonly ISessionStorageService iss;
		public Data.Session Session { get; private set; } = new Data.Session();
		public string? Name { get; private set; }
		public SessionService(ISessionStorageService iss) => this.iss = iss;
		public async Task LoadAsync() => Session = await iss.GetItemAsync<Data.Session?>("session") ?? new Data.Session();
		public Task SetAsync(Data.Session? session, string? name = null)
		{
			Name = name;
			Session = session ?? new Data.Session();
			return !IsSet ? iss.RemoveItemAsync("session") : iss.SetItemAsync("session", Session);
		}
		public string Key => Session.Key;
		public int GameId => Session.G;
		public int Player => Session.P;
		public bool IsSet => Session.IsSet();
	}
}
