using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayer : IEntity
	{
		[Key] public int Id { get; set; }
		public EntityGame EntityGame { get; set; } = new EntityGame();
		public int EntityGameId { get; set; }
		public int Index { get; set; }
		public string Name { get; set; } = "";
		public string Color { get; set; } = "";
		public string Type { get; set; } = "";
		public string Password { get; set; } = "";
		public int Money { get; set; }
		public bool Alive { get; set; }
		public IEnumerable<EntityPlayerAction>? EntityPlayerActions { get; set; }
		public Player Convert(Settings settings) => Type switch
		{
			"H" => new Human(Shared.State.Color.Parse(Color), Name, Money, Alive, EntityPlayerActions!.Select(a => a.Convert(settings)).ToImmutableList(), Shared.State.Password.Parse(Password)),
			"R" => new Robot(Shared.State.Color.Parse(Color), Money, Alive, EntityPlayerActions!.Select(a => a.Convert(settings)).ToImmutableList(), settings),
			"S" => new Savage(),
			_ => throw new Exception("Unknown Player type: " + Type)
		};
		public EntityPlayer Assign(Player p)
		{
			_ = p switch
			{
				Human H => (Type = "H", Color = H.Color.ToString(), Name = H.Name, Money = H.Money, Alive = H.Alive, EntityPlayerActions = H.Actions.Select(EntityPlayerAction.From), Password = H.Password.ToString()),
				Robot R => (Type = "R", Color = R.Color.ToString(), Name = R.Name, Money = R.Money, Alive = R.Alive, EntityPlayerActions = R.Actions.Select(EntityPlayerAction.From)),
				_ => (object)(Type = "S"),
			};
			return this;
		}
		public static EntityPlayer From(Player p, int index, int gameId) => new EntityPlayer { Index = index, EntityGameId = gameId }.Assign(p);
	}
}