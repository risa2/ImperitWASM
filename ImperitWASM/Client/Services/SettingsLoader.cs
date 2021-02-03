using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;

namespace ImperitWASM.Client.Services
{
	public class SettingsLoader
	{
		readonly IClient http;
		public ImmutableArray<SoldiersItem> Types { get; private set; }
		public SettingsLoader(IClient http) => this.http = http;
		public async Task LoadAsync(string typesUrl)
		{
			if (Types.IsDefaultOrEmpty)
			{
				Types = await http.GetAsync<ImmutableArray<SoldiersItem>>(typesUrl);
			}
		}
	}
}