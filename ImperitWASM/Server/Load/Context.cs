using Microsoft.EntityFrameworkCore;

namespace ImperitWASM.Server.Load
{
	public class Context : DbContext
	{
		public DbSet<EntitySession>? EntitySessions { get; set; }
		public DbSet<Game>? Games { get; set; }
		public DbSet<EntityPlayer>? EntityPlayers { get; set; }
		public DbSet<EntityProvince>? EntityProvinces { get; set; }
		public DbSet<EntityPlayerPower>? EntityPlayerPowers { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder opt) => opt.UseSqlite("Data Source=Files/imperit.db");
		protected override void OnModelCreating(ModelBuilder mod)
		{
			_ = mod.OneToMany<Game, EntitySession>(x => x.Game, x => x.EntitySessions).IsRequired();
			_ = mod.OneToMany<Game, EntityPlayer>(x => x.Game, x => x.EntityPlayers).IsRequired();
			_ = mod.OneToMany<Game, EntityProvince>(x => x.Game, x => x.EntityProvinces).IsRequired();
			_ = mod.OneToMany<Game, EntityPlayerPower>(x => x.Game, x => x.EntityPlayerPowers).IsRequired();

			_ = mod.OneToMany<EntityPlayer, EntityPlayerAction>(x => x.EntityPlayer, x => x.EntityPlayerActions).IsRequired();
			_ = mod.OneToMany<EntityPlayer, EntityProvince>(x => x.EntityPlayer).IsRequired();
			_ = mod.OneToMany<EntityPlayer, EntityProvinceAction>(x => x.EntityPlayer).NotRequired();
			_ = mod.OneToMany<EntityProvince, EntityProvinceAction>(x => x.EntityProvince, x => x.EntityProvinceActions).IsRequired();
			_ = mod.OneToOne<EntityProvince, EntitySoldier>(x => x.EntitySoldier).IsRequired();
			_ = mod.OneToOne<EntityProvinceAction, EntitySoldier>(x => x.EntitySoldier).NotRequired();
		}
	}
}
