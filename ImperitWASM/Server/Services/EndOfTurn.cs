using System.Linq;
using ImperitWASM.Shared.Commands;
using ImperitWASM.Shared.Config;

namespace ImperitWASM.Server.Services
{
	public interface IEndOfTurn
	{
		bool NextTurn(int gameId, int player);
	}
	public class EndOfTurn : IEndOfTurn
	{
		readonly IPowersLoader power_load;
		readonly IGameLoader game_load;
		readonly ICommandExecutor cmd;
		readonly Settings settings;
		readonly IDatabase db;
		public EndOfTurn(Settings settings, ICommandExecutor cmd, IGameLoader game_load, IDatabase db, IPowersLoader power_load)
		{
			this.power_load = power_load;
			this.game_load = game_load;
			this.settings = settings;
			this.cmd = cmd;
			this.db = db;
		}
		public bool NextTurn(int gameId, int player) => db.Transaction(true, () =>
		{
			var (_, players, provinces) = cmd.Perform(gameId, player, new NextTurn(), true);
			power_load.Add(gameId, players.Select(p => p.Power(provinces)), true);

			bool finish = players.Any(p => p is { LivingHuman: true }) || provinces.Winner.Item2 >= settings.FinalLandsCount;
			if (finish)
			{
				game_load[gameId] = game_load[gameId]!.Finish();
			}
			return finish;
		});
	}
}
