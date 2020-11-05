using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Load
{
	public class Context : DbContext
	{
		public DbSet<EntitySession>? EntitySessions { get; set; }
		public DbSet<EntityGame>? EntityGames { get; set; }
		public DbSet<EntityPlayer>? EntityPlayers { get; set; }
		public DbSet<EntityProvince>? EntityProvinces { get; set; }
		public DbSet<EntityPlayerPower>? EntityPlayerPowers { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder opt)
		{

		}
		protected override void OnModelCreating(ModelBuilder mod)
		{
			
		}
	}
}
