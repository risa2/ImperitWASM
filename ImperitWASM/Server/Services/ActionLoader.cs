using ImperitWASM.Server.Load;
using ImperitWASM.Shared.Motion;
using ImperitWASM.Shared.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImperitWASM.Server.Services
{
	public interface IActionLoader : IEnumerable<IAction>
	{
		bool Add(IEnumerable<ICommand> commands);
		bool Add(ICommand command) => Add(new[] { command });
		void EndOfTurn(int active);
		void Save();
		void Save(ActionQueue Actions);
	}
	public class ActionLoader : IActionLoader
	{
		readonly IPlayersLoader players;
		readonly IProvincesLoader provinces;
		readonly JsonWriter<JsonAction, IAction, (Settings, IReadOnlyList<Player>)> loader;
		ActionQueue actions;
		public ActionLoader(ISettingsLoader sl, IPlayersLoader players, IProvincesLoader provinces, IServiceIO io)
		{
			this.players = players;
			this.provinces = provinces;
			loader = new JsonWriter<JsonAction, IAction, (Settings, IReadOnlyList<Player>)>(io.Actions, (sl.Settings, players), JsonAction.From);
			actions = new ActionQueue(loader.Load());
		}
		public bool Add(IEnumerable<ICommand> commands)
		{
			bool changed = false;
			foreach (var command in commands)
			{
				var (queue, new_players, new_provinces, ch) = actions.Add(command, players, provinces);
				actions = queue;
				if (ch)
				{
					provinces.Set(new_provinces);
					players.Set(new_players);
					changed = true;
				}
			}
			return changed;
		}
		public void Save(ActionQueue queue) => loader.Save(actions = queue);
		public ActionQueue Where(Func<IAction, bool> cond) => new ActionQueue(actions.Where(cond));
		public void EndOfTurn(int active)
		{
			var (queue, new_players, new_provinces) = actions.EndOfTurn(players, provinces, active);
			actions = queue;
			provinces.Set(new_provinces.ToArray());
			players.Set(new_players);
		}
		public void Save() => Save(actions);

		public IEnumerator<IAction> GetEnumerator() => actions.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}