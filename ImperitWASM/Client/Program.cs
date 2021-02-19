using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ImperitWASM.Client
{
	public class Program
	{
		public static Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");

			_ = builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }).AddBlazoredSessionStorage()
					.AddScoped<Services.IClient, Services.ImperitClient>().AddScoped<Services.SessionService>()
					.AddScoped<Services.SettingsLoader>().AddScoped<Services.ProvinceContainer>();
			return builder.Build().RunAsync();
		}
	}
}
