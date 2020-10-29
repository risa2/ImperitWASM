using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Load
{
	public class Context : DbContext
	{
		public DbSet<EntityGame>? EntityGames { get; set; }
		public DbSet<EntitySession>? EntitySessions { get; set; }
		public DbSet<EntityPlayer>? EntityPlayers { get; set; }
		public DbSet<EntityProvince>? Provinces { get; set; }
		public DbSet<EntityPlayerPower>? PlayersPowers { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
		}
	}
}
