using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;
using ImperitWASM.Shared.Data;

namespace ImperitWASM.Server.Services
{
	public interface ICommandExecutor
	{
		(bool, ImmutableArray<Player>, Provinces, Game?) Perform(int gameId, int player, ICommand command, ImmutableArray<Player> players, Provinces provinces);
		(bool, ImmutableArray<Player>, Provinces, Game?) Perform(int gameId, int player, ICommand command, bool fromTransaction);
	}
	public class CommandExecutor : ICommandExecutor
	{
		readonly IDatabase db;
		readonly IPlayerLoader player_load;
		readonly IProvinceLoader province_load;
		readonly Settings settings;
		readonly IPowersLoader power_load;
		readonly IGameLoader game_load;
		public CommandExecutor(IDatabase db, IPlayerLoader player_load, IProvinceLoader province_load, Settings settings, IPowersLoader power_load, IGameLoader game_load)
		{
			this.db = db;
			this.player_load = player_load;
			this.province_load = province_load;
			this.settings = settings;
			this.power_load = power_load;
			this.game_load = game_load;
		}

		public (bool, ImmutableArray<Player>, Provinces, Game?) Perform(int gameId, int player, ICommand command, ImmutableArray<Player> players, Provinces provinces)
		{
			var game = game_load[gameId];
			if (game is not null && command.Allowed(players[player], players, provinces, settings))
			{
				var (new_pl, new_pr, new_game) = command.Perform(players[player], players, provinces, settings, game);
				var (new_players, new_provinces) = (new_pl.ToImmutableArray(), provinces.With(new_pr));

				game_load[gameId] = new_game;
				power_load.Add(gameId, new_players.Select(p => p.Power(new_provinces)), true);
				player_load.Set(gameId, new_players, true);
				province_load.Set(gameId, new_provinces, true);
				return (true, new_players, new_provinces, new_game);
			}
			return (false, players, provinces, game);
		}

		public (bool, ImmutableArray<Player>, Provinces, Game?) Perform(int gameId, int player, ICommand command, bool fromTransaction)
		{
			return db.Transaction(!fromTransaction, () => Perform(gameId, player, command, player_load[gameId], province_load[gameId]));
		}
	}
}
