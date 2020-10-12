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
			return await JsonSerializer.DeserializeAsync<TResponse>(await http.GetStreamAsync(url), opt);
		}
		public static async Task<HttpContent> PostJsonAsync<TData>(this HttpClient http, string url, TData data)
		{
			return (await http.PostAsync(url, new StringContent(JsonSerializer.Serialize(data, opt), Encoding.UTF8, "application/json"))).Content;
		}
		public static async Task<TResponse> PostJsonResponseAsync<TData, TResponse>(this HttpClient http, string url, TData data)
		{
			return await JsonSerializer.DeserializeAsync<TResponse>(await (await http.PostJsonAsync(url, data)).ReadAsStreamAsync(), opt);
		}
	}
}
