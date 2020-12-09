using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImperitWASM.Shared.Actions;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Server.Load
{
	public class EntityPlayer : IEntity
	{
		public enum Kind { Human, Savage, Robot }
		[Key] public int Id { get; set; }
		public Game? Game { get; set; }
		public int GameId { get; set; }
		public int Index { get; set; }
		public string Name { get; set; } = "";
		public string Color { get; set; } = "";
		public Kind Type { get; set; }
		public string Password { get; set; } = "";
		public int Money { get; set; }
		public bool Alive { get; set; }
		public ICollection<EntityPlayerAction>? EntityPlayerActions { get; set; }
		ImmutableList<IPlayerAction> GetActions(Settings settings) => EntityPlayerActions!.OrderBy(a => a.Type).Select(a => a.Convert(settings)).ToImmutableList();
		public Player Convert(Settings settings) => Type switch
		{
			Kind.Human => new Human(Shared.State.Color.Parse(Color), Name, Money, Alive, GetActions(settings), settings, Shared.State.Password.Parse(Password)),
			Kind.Robot => new Robot(Shared.State.Color.Parse(Color), Name, Money, Alive, GetActions(settings), settings),
			_ => settings.Savage,
		};
		public static EntityPlayer From(Player p, int index) => p switch
		{
			Human H => new EntityPlayer { Index = index, Type = Kind.Human, Color = H.Color.ToString(), Name = H.Name, Money = H.Money, Alive = H.Alive, EntityPlayerActions = H.Actions.Select(EntityPlayerAction.From).ToList(), Password = H.Password.ToString() },
			Robot R => new EntityPlayer { Index = index, Type = Kind.Robot, Color = R.Color.ToString(), Name = R.Name, Money = R.Money, Alive = R.Alive, EntityPlayerActions = R.Actions.Select(EntityPlayerAction.From).ToList() },
			_ => new EntityPlayer { Index = index, Type = Kind.Savage },
		};
	}
}