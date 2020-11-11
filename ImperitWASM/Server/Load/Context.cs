using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Load
{
	public class Context : DbContext
	{
		public DbSet<EntitySession>? EntitySessions { get; set; }
		public DbSet<Game>? EntityGames { get; set; }
		public DbSet<EntityPlayer>? EntityPlayers { get; set; }
		public DbSet<EntityProvince>? EntityProvinces { get; set; }
		public DbSet<EntityPlayerPower>? EntityPlayerPowers { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder opt)
		{
			_ = opt.UseSqlite("Data Source=Files/imperit.db");
		}
		protected override void OnModelCreating(ModelBuilder mod)
		{
			
		}
	}
}
