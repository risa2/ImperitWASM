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
			_ = services.AddSingleton<IConfig, Config>(s => new Config(File.Path(p, "Files/Shapes.json"), File.Path(p, "Files/Graph.json"), File.Path(p, "Files/Settings.json")))			
					.AddSingleton<ISessionService, SessionService>().AddSingleton<IPlayersProvinces, PlayersProvinces>()
					.AddSingleton<IContextService, ContextService>().AddSingleton<IPowers, Powers>()
					.AddSingleton<IGameService, GameService>().AddTransient<INewGame, GameCreator>()
					.AddTransient<IEndOfTurn, EndOfTurn>().AddSingleton<IActive, Active>();
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
