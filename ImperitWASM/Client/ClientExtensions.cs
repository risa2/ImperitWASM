using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using Blazored.SessionStorage;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Client
{
	internal class HttpNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName(string name) => new string(name[0], 1).ToLower() + name[1..];
		public static readonly JsonNamingPolicy Instance = new HttpNamingPolicy();
	}
	public static class ClientExtensions
	{
		public static IEnumerable<T> Reverse<T>(this IEnumerable<T> e, bool reverse) => reverse ? e.Reverse() : e;
		public static IEnumerable<(int i, T v)> Index<T>(this IEnumerable<T> e) => e.Select((v, i) => (i, v));
		public static IEnumerable<T> Try<T>(this IEnumerable<T>? e) => e is null ? Enumerable.Empty<T>() : e;
		static readonly JsonSerializerOptions opt = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = HttpNamingPolicy.Instance
		};
		static async Task<T> Parse<T>(HttpResponseMessage msg)
		{
			return await JsonSerializer.DeserializeAsync<T>(await msg.Content.ReadAsStreamAsync(), opt);
		}
		static StringContent MakeContent<T>(T data)
		{
			return new StringContent(JsonSerializer.Serialize(data, opt), Encoding.UTF8, "application/json");
		}
		public static async Task<T> GetJsonAsync<T>(this HttpClient http, string url)
		{
			return await Parse<T>(await http.GetAsync(url));
		}
		public static Task PostJsonAsync<T>(this HttpClient http, string url, T data)
		{
			return http.PostAsync(url, MakeContent(data));
		}
		public static async Task<T> PostJsonResponseAsync<TD, T>(this HttpClient http, string url, TD data)
		{
			return await Parse<T>(await http.PostAsync(url, MakeContent(data)));
		}
		public static Task<string?> GetStringAsync(this ISessionStorageService cookies, string key)
		{
			return cookies.GetItemAsync<string?>(key);
		}
	}
}
