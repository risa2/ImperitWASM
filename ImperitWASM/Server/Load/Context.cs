using Microsoft.EntityFrameworkCore;
using ImperitWASM.Shared.Entities;

namespace ImperitWASM.Server.Load
{
	public class Context : DbContext
	{
		public DbSet<Session>? Sessions { get; set; }
		public DbSet<Game>? Games { get; set; }
		public DbSet<Player>? Players { get; set; }
		public DbSet<Province>? Provinces { get; set; }
		public DbSet<PlayerPower>? PlayerPowers { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder opt)
		{

		}
		protected override void OnModelCreating(ModelBuilder mod)
		{
			
		}
	}
}
