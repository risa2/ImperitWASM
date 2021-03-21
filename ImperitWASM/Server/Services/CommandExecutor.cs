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
		readonly IPowerLoader power_load;
		readonly IGameLoader game_load;
		public CommandExecutor(IDatabase db, IPlayerLoader player_load, IProvinceLoader province_load, Settings settings, IPowerLoader power_load, IGameLoader game_load)
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
			if (game is { Current: Game.State.Started } && command.Allowed(players[player], players, provinces, settings))
			{
				var (players_en, provinces_en, new_game, powers) = command.Perform(players[player], players, provinces, settings, game);
				var (new_players, new_provinces) = (players_en.ToImmutableArray(), provinces.With(provinces_en));

				game_load[gameId] = new_game;
				power_load.Add(gameId, powers.SelectMany(p => p.Items), true);
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
