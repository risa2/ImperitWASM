using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class JsonPlayer : IEntity<Player, Settings>
	{
		public string Name { get; set; } = "";
		public Color Color { get; set; }
		public string Type { get; set; } = "";
		public string Password { get; set; } = "";
		public int Money { get; set; }
		public bool Alive { get; set; }
		public IEnumerable<JsonPlayerAction>? Actions { get; set; }
		public Player Convert(int i, Settings settings) => Type switch
		{
			"H" => new Human(i, Color, Money, Alive, Actions!.Select((a, i) => a.Convert(i, settings)).ToImmutableList(), Name, Shared.State.Password.Parse(Password)),
			"R" => new Robot(i, Color, Money, Alive, Actions!.Select((a, i) => a.Convert(i, settings)).ToImmutableList(), settings),
			"S" => new Savage(i),
			_ => throw new Exception("Unknown Player type: " + Type)
		};
		public static JsonPlayer From(Player p) => p switch
		{
			Human H => new JsonPlayer { Name = H.Name, Color = H.Color, Type = "H", Money = H.Money, Alive = H.Alive, Actions = H.Actions.Select(JsonPlayerAction.From), Password = H.Password.ToString() },
			Robot R => new JsonPlayer { Color = R.Color, Type = "R", Money = R.Money, Alive = R.Alive, Actions = R.Actions.Select(JsonPlayerAction.From) },
			_ => new JsonPlayer { Type = "S" },
		};
	}
}