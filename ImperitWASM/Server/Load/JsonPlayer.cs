using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
			"S" => new Savage(i),
			"R" => new Robot(i, Name, Color, Shared.State.Password.Parse(Password), Money, Alive, settings, Actions!.Select((a, i) => a.Convert(i, settings)).ToImmutableList()),
			"P" => new Player(i, Name, Color, Shared.State.Password.Parse(Password), Money, Alive, Actions!.Select((a, i) => a.Convert(i, settings)).ToImmutableList()),
			_ => throw new Exception("Unknown Player type: " + Type)
		};
		public static JsonPlayer From(Player p) => p switch
		{
			Savage _ => new JsonPlayer { Type = "S" },
			Robot R => new JsonPlayer { Name = R.Name, Color = R.Color, Type = "R", Password = R.Password.ToString(), Money = R.Money, Alive = R.Alive, Actions = R.Actions.Select(JsonPlayerAction.From) },
			Player P => new JsonPlayer { Name = P.Name, Color = P.Color, Type = "P", Password = P.Password.ToString(), Money = P.Money, Alive = P.Alive, Actions = P.Actions.Select(JsonPlayerAction.From) },
		};
	}
}