using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayer : IEntity<Player, Settings>
	{
		[Key] public int Id { get; set; }
		[Required] public int GameId { get; set; }
		[Required] public int Index { get; set; }
		[Required] public string Name { get; set; } = "";
		[Required] public string Color { get; set; } = "";
		[Required] public string Type { get; set; } = "";
		[Required] public string Password { get; set; } = "";
		[Required] public int Money { get; set; }
		[Required] public bool Alive { get; set; }
		public IEnumerable<EntityPlayerAction>? Actions { get; set; }
		public Player Convert(Settings settings) => Type switch
		{
			"H" => new Human(Shared.State.Color.Parse(Color), Name, Money, Alive, Actions!.Select(a => a.Convert(settings)).ToImmutableList(), Shared.State.Password.Parse(Password)),
			"R" => new Robot(Shared.State.Color.Parse(Color), Money, Alive, Actions!.Select(a => a.Convert(settings)).ToImmutableList(), settings),
			"S" => new Savage(),
			_ => throw new Exception("Unknown Player type: " + Type)
		};
		public EntityPlayer Assign(Player p)
		{
			_ = p switch
			{
				Human H => (Type = "H", Color = H.Color.ToString(), Name = H.Name, Money = H.Money, Alive = H.Alive, Actions = H.Actions.Select(EntityPlayerAction.From), Password = H.Password.ToString()),
				Robot R => (Type = "R", Color = R.Color.ToString(), Name = R.Name, Money = R.Money, Alive = R.Alive, Actions = R.Actions.Select(EntityPlayerAction.From)),
				_ => (object)(Type = "S"),
			};
			return this;
		}
		public static EntityPlayer From(Player p, int index, int gameId) => new EntityPlayer { Index = index, GameId = gameId }.Assign(p);
	}
}