using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;

namespace ImperitWASM.Client.Services
{
	public class SettingsLoader
	{
		readonly ImperitClient http;
		public ImmutableArray<SoldiersItem> Types { get; private set; }
		public SettingsLoader(ImperitClient http) => this.http = http;
		public async Task LoadAsync(string typesUrl)
		{
			if (Types.IsDefaultOrEmpty)
			{
				Types = await http.GetAsync<ImmutableArray<SoldiersItem>>(typesUrl);
			}
		}
	}
}