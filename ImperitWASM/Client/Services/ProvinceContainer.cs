using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImperitWASM.Client.Data;
using ImperitWASM.Shared;

namespace ImperitWASM.Client.Services
{
	public class ProvinceContainer : IReadOnlyList<ProvinceDisplay>
	{
		readonly IClient http;
		List<ProvinceDisplay>? provinces;
		public ProvinceContainer(IClient http) => this.http = http;

		public ProvinceDisplay this[int index] => provinces![index];
		public int Count => provinces!.Count;
		public IEnumerator<ProvinceDisplay> GetEnumerator() => provinces!.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => provinces!.GetEnumerator();

		public async Task LoadAsync(string url) => provinces ??= await http.GetAsync<List<ProvinceDisplay>>(url);
		public async Task UpdateAsync(string url, int gameId)
		{
			if (provinces is not null)
			{
				var values = await http.PostAsync<int, ImmutableArray<ProvinceUpdate>>(url, gameId);
				values.Each((value, i) => provinces[i] = provinces[i].Update(value));
			}
		}
	}
}