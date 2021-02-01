using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface ICommandExecutor
	{
		(bool, ImmutableArray<Player>, Provinces) Perform(int gameId, ICommand command, bool fromTransaction);
	}
	public class CommandExecutor : ICommandExecutor
	{
		readonly IDatabase db;
		readonly IPlayerLoader player_load;
		readonly IProvinceLoader province_load;
		readonly Settings settings;
		public CommandExecutor(IDatabase db, IPlayerLoader player_load, IProvinceLoader province_load, Settings settings)
		{
			this.db = db;
			this.player_load = player_load;
			this.province_load = province_load;
			this.settings = settings;
		}

		public (bool, ImmutableArray<Player>, Provinces) Perform(int gameId, ICommand command, bool fromTransaction) => db.Transaction(!fromTransaction, () =>
		{
			var players = player_load[gameId];
			var provinces = province_load.Get(gameId, players);
			var player = players.First(p => p.Active);
			if (command.Allowed(player, players, provinces, settings))
			{
				var (new_player_e, new_province_e) = command.Perform(player, players, provinces, settings);
				var (new_players, new_provinces) = (new_player_e.ToImmutableArray(), provinces.With(new_province_e));

				player_load.Set(gameId, new_players, true);
				province_load.Set(gameId, new_provinces, true);
				return (true, new_players, new_provinces);
			}
			return (false, players, provinces);
		});
	}
}
