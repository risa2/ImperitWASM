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
			_ = services.AddControllersWithViews();
			_ = services.AddRazorPages();
			_ = services.AddDbContext<Context>()
					.AddSingleton<IConfig, Config>(s => new Config(new File(System.AppDomain.CurrentDomain.BaseDirectory ?? ".", "Files/Settings.json")))
					.AddTransient<IContextService, ContextService>().AddTransient<ISessionService, SessionService>()
					.AddTransient<IPlayersProvinces, PlayersProvinces>().AddTransient<IPowers, Powers>()
					.AddTransient<IGameService, GameService>().AddTransient<IActive, Active>()
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
