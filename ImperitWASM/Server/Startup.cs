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
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddControllersWithViews();
			services.AddRazorPages();
			services.AddSingleton<IServiceIO>(s => new ServiceIO(new File("./Files/Settings.json"), new File("./Files/Players.json"), new File("./Files/Provinces.json"), new File("./Files/Actions.json"), new File("./Files/Events.json"), new File("./Files/Active.json"), new File("./Files/Password.txt"), new File("./Files/Graph.json"), new File("./Files/Mountains.json"), new File("./Files/Shapes.json"), new File("./Files/Powers.json"), new File("./Files/Game.json"), new File("./Files/FormerPlayers.json")))
					.AddSingleton<ILoginService, LoginService>().AddSingleton<IActionLoader, ActionLoader>()
					.AddSingleton<ISettingsLoader, SettingsLoader>().AddSingleton<IPlayersLoader, PlayersLoader>()
					.AddSingleton<IFormerPlayersLoader, FormerPlayersLoader>()
					.AddSingleton<IProvincesLoader, ProvincesLoader>().AddSingleton<IPowersLoader, PowersLoader>()
					.AddSingleton<IGameLoader, GameLoader>().AddTransient<IActivePlayer, ActivePlayer>()
					.AddTransient<INewGame, NewGame>().AddTransient<IEndOfTurn, EndOfTurn>();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
