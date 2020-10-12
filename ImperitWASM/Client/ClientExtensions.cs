using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

namespace ImperitWASM.Client
{
	internal class HttpNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName(string name) => new string(name[0], 1).ToLower() + name[1..];
		public static readonly JsonNamingPolicy Instance = new HttpNamingPolicy();
	}
	public static class ClientExtensions
	{
		static readonly JsonSerializerOptions opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = HttpNamingPolicy.Instance };
		public static async Task<TResponse> GetJsonAsync<TResponse>(this HttpClient http, string url)
		{
			System.Console.WriteLine("GET <" + url + "> response: " + await http.GetStringAsync(url));
			return await JsonSerializer.DeserializeAsync<TResponse>(await http.GetStreamAsync(url), opt);
		}
		public static async Task<HttpContent> PostJsonAsync<TData>(this HttpClient http, string url, TData data)
		{
			System.Console.WriteLine("POST <" + url + "> body: " + JsonSerializer.Serialize(data, opt));
			return (await http.PostAsync(url, new StringContent(JsonSerializer.Serialize(data, opt), Encoding.UTF8, "application/json"))).Content;
		}
		public static async Task<TResponse> PostJsonResponseAsync<TData, TResponse>(this HttpClient http, string url, TData data)
		{
			var p = await http.PostJsonAsync(url, data);
			System.Console.WriteLine("POST <" + url + "> response: " + await p.ReadAsStringAsync());
			return await JsonSerializer.DeserializeAsync<TResponse>(await p.ReadAsStreamAsync(), opt);
		}
	}
}
