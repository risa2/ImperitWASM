using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImperitWASM.Client.Services
{
	public class ImperitClient
	{
		private class Policy : JsonNamingPolicy
		{
			public override string ConvertName(string name) => new string(name[0], 1).ToLower() + name[1..];
		}

		private readonly HttpClient http;
		public ImperitClient(HttpClient http) => this.http = http;

		private static readonly JsonSerializerOptions opt = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = new Policy()
		};

		private static async Task<T> Parse<T>(HttpResponseMessage msg)
		{
			return (await JsonSerializer.DeserializeAsync<T>(await msg.Content.ReadAsStreamAsync(), opt))!;
		}

		private static StringContent MakeContent<T>(T data)
		{
			return new StringContent(JsonSerializer.Serialize(data, opt), Encoding.UTF8, "application/json");
		}
		public async Task<T> GetJsonAsync<T>(string url) => await Parse<T>(await http.GetAsync(url));
		public Task PostJsonAsync<T>(string url, T data) => http.PostAsync(url, MakeContent(data));
		public async Task<T> PostJsonResponseAsync<TD, T>(string url, TD data)
		{
			return await Parse<T>(await http.PostAsync(url, MakeContent(data)));
		}
	}
}
