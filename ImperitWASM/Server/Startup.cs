using ImperitWASM.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ImperitWASM.Server
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public Startup(IConfiguration configuration) => Configuration = configuration;
		public void ConfigureServices(IServiceCollection services)
		{
			_ = services.AddControllersWithViews();
			_ = services.AddRazorPages();
			string folder = System.AppDomain.CurrentDomain.BaseDirectory ?? ".";
			_ = services.AddScoped<IDatabase, SqliteDatabase>(_ => new SqliteDatabase(Path.Combine(folder, "Files/imperit.db")))
					.AddSingleton(s => Config.Load(folder, "Files/Settings.json"))
					.AddScoped<ISessionLoader, SessionLoader>().AddScoped<IProvinceLoader, ProvinceLoader>()
					.AddScoped<IGameLoader, GameLoader>().AddScoped<IPlayerLoader, PlayerLoader>()
					.AddScoped<IPowerLoader, PowerLoader>().AddScoped<IGameCreator, GameCreator>()
					.AddScoped<ICommandExecutor, CommandExecutor>();
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			_ = app.UseExceptionHandler("/Error").UseHsts().UseHttpsRedirection().UseBlazorFrameworkFiles().UseStaticFiles().UseRouting().UseEndpoints(endpoints =>
			{
				_ = endpoints.MapRazorPages();
				_ = endpoints.MapControllers();
				_ = endpoints.MapFallbackToFile("index.html");
				_ = endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
