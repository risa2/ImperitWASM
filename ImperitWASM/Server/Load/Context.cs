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
			_ = mod.OneToMany<Game, EntitySession>(x => x.GameId).IsRequired();
			_ = mod.OneToMany<Game, EntityPlayer>(x => x.GameId).IsRequired();
			_ = mod.OneToMany<Game, EntityProvince>(x => x.GameId).IsRequired();
			_ = mod.OneToMany<Game, EntityPlayerPower>(x => x.GameId).IsRequired();
			_ = mod.OneToMany<EntityPlayer, EntityPlayerAction>(x => x.EntityPlayerId).IsRequired();
			_ = mod.OneToMany<EntityProvince, EntityProvinceAction>(x => x.EntityProvinceId).IsRequired();
			_ = mod.OneToOne<EntitySoldier, EntityProvince>(x => x.EntitySoldierId).IsRequired();
			_ = mod.OneToOne<EntitySoldier, EntityProvinceAction>(x => x.EntitySoldierId);
		}
	}
}
