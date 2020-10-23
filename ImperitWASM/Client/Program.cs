using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImperitWASM.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			_ = builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
								.AddScoped<Services.ImperitClient>()
								.AddBlazoredSessionStorage()
								.AddScoped(provider => new Services.SessionService(provider.GetRequiredService<ISessionStorageService>(), "id", "login"));
			await builder.Build().RunAsync();
		}
	}
}
