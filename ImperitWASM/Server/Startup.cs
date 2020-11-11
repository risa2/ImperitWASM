using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ImperitWASM.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }
		public void ConfigureServices(IServiceCollection services)
		{
			string p = System.AppDomain.CurrentDomain.BaseDirectory ?? ".";
			_ = services.AddControllersWithViews();
			_ = services.AddRazorPages();
			_ = services.AddSingleton<IConfig, Config>(s => new Config(new File(p, "Files/Settings.json")))
					.AddSingleton<ISessionService, SessionService>().AddSingleton<IContextService, ContextService>()
					.AddSingleton<IPlayersProvinces, PlayersProvinces>().AddSingleton<IPowers, Powers>()
					.AddSingleton<IGameService, GameService>().AddSingleton<IActive, Active>()
					.AddTransient<INewGame, GameCreator>().AddTransient<IEndOfTurn, EndOfTurn>();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			_ = (env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseExceptionHandler("/Error").UseHsts()).UseHttpsRedirection().UseBlazorFrameworkFiles().UseStaticFiles().UseRouting().UseEndpoints(endpoints =>
			{
				_ = endpoints.MapRazorPages();
				_ = endpoints.MapControllers();
				_ = endpoints.MapFallbackToFile("index.html");
				_ = endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
