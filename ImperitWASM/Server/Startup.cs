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
			_ = services.AddSingleton<IServiceIO>(s => new ServiceIO(File.Path(p, "Files/Settings.json"), File.Path(p, "Files/Players.json"), File.Path(p, "Files/Provinces.json"), File.Path(p, "Files/Active.json"), File.Path(p, "Files/Graph.json"), File.Path(p, "Files/Shapes.json"), File.Path(p, "Files/Powers.json"), File.Path(p, "Files/Game.json"), File.Path(p, "Files/FormerPlayers.json"), File.Path(p, "Files/Sessions.json")))
					.AddSingleton<ILoginService, LoginService>().AddSingleton<ISettingsLoader, SettingsLoader>()
					.AddSingleton<IPlayersProvinces, PlayersProvinces>().AddSingleton<IFormerPlayers, FormerPlayers>()
					.AddSingleton<IPowersLoader, PowersLoader>().AddSingleton<IGameLoader, GameLoader>()
					.AddTransient<INewGame, NewGame>().AddTransient<IEndOfTurn, EndOfTurn>();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage().UseWebAssemblyDebugging();
			}
			else
			{
				_ = app.UseExceptionHandler("/Error").UseHsts();
			}

			_ = app.UseHttpsRedirection().UseBlazorFrameworkFiles().UseStaticFiles().UseRouting();

			_ = app.UseEndpoints(endpoints =>
			{
				_ = endpoints.MapRazorPages();
				_ = endpoints.MapControllers();
				_ = endpoints.MapFallbackToFile("index.html");
				_ = endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
