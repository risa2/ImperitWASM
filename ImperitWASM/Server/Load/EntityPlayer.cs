using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.Conversion;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayer : IEntity<Player, Settings>
	{
		[Key] public int Id { get; set; }
		public int Index { get; set; }
		public string Name { get; set; } = "";
		public Color Color { get; set; }
		public string Type { get; set; } = "";
		public string Password { get; set; } = "";
		public int Money { get; set; }
		public bool Alive { get; set; }
		public IEnumerable<EntityPlayerAction>? Actions { get; set; }
		public Player Convert(Settings settings) => Type switch
		{
			"H" => new Human(Index, Color, Money, Alive, Actions!.Select(a => a.Convert(settings)).ToImmutableList(), Name, Shared.State.Password.Parse(Password)),
			"R" => new Robot(Index, Color, Money, Alive, Actions!.Select(a => a.Convert(settings)).ToImmutableList(), settings),
			"S" => new Savage(Index),
			_ => throw new Exception("Unknown Player type: " + Type)
		};
		public static EntityPlayer From(Player p) => p switch
		{
			Human H => new EntityPlayer { Type = "H", Index = p.Id, Name = H.Name, Color = H.Color, Money = H.Money, Alive = H.Alive, Actions = H.Actions.Select(EntityPlayerAction.From), Password = H.Password.ToString() },
			Robot R => new EntityPlayer { Type = "R", Index = p.Id, Color = R.Color, Money = R.Money, Alive = R.Alive, Actions = R.Actions.Select(EntityPlayerAction.From) },
			_ => new EntityPlayer { Type = "S", Index = p.Id },
		};
	}
}