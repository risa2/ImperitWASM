using ImperitWASM.Server.Load;
using ImperitWASM.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			_ = services.AddScoped<IDatabase, SqliteDatabase>(_ => new SqliteDatabase("Files/imperit.db"))
					.AddSingleton(s => Config.Load(System.AppDomain.CurrentDomain.BaseDirectory ?? ".", "Files/Settings.json"))
					.AddScoped<ISessionService, SessionService>().AddScoped<IProvinceLoader, ProvinceLoader>()
					.AddScoped<IGameCreator, GameCreator>().AddScoped<IEndOfTurn, EndOfTurn>();
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
