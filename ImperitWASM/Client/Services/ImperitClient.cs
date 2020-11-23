using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImperitWASM.Client.Services
{
	public class ImperitClient
	{
		class Policy : JsonNamingPolicy
		{
			public override string ConvertName(string name) => new string(name[0], 1).ToLower() + name[1..];
		}

		readonly HttpClient http;
		public ImperitClient(HttpClient http) => this.http = http;

		static readonly JsonSerializerOptions opt = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = new Policy()
		};

		static async Task<T> Parse<T>(HttpResponseMessage msg) => (await JsonSerializer.DeserializeAsync<T>(await msg.Content.ReadAsStreamAsync(), opt))!;
		static StringContent MakeContent<T>(T data) => new StringContent(JsonSerializer.Serialize(data, opt), Encoding.UTF8, "application/json");
		public async Task<T> GetAsync<T>(string url) => await Parse<T>(await http.GetAsync(url));
		public Task PostAsync<T>(string url, T data) => http.PostAsync(url, MakeContent(data));
		public async Task<TR> PostAsync<T, TR>(string url, T data) => await Parse<TR>(await http.PostAsync(url, MakeContent(data)));
	}
}
